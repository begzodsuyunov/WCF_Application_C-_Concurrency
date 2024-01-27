using ClientApp.FlightService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        private string filePath;
        private Semaphore gateSemaphore = new Semaphore(10, 10);
        DataTable flightTable = new DataTable();
        List<Gate> gates;
        private Semaphore runwaySemaphore = new Semaphore(3, 3);
        private List<Flight> flights = new List<Flight>();
        private static readonly object locker = new object();
        private readonly object gatesLocker = new object();

        public Form1()
        {
            InitializeComponent();
            InitTimer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //For getting the location of the Flight.txt file
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                this.Form1_Load(sender, e);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gates = new List<Gate> { new Gate { number = "A" }, new Gate { number = "B" }, new Gate { number = "C" }, new Gate { number = "D" }, new Gate { number = "E" }, new Gate { number = "F" }, new Gate { number = "G" }, new Gate { number = "H" }, new Gate { number = "I" }, new Gate { number = "J" } };

            //Grid view column naming
            if (!flightTable.Columns.Contains("Flight Code"))
            {
                flightTable.Columns.Add("Flight Code", typeof(string));
            }
            if (!flightTable.Columns.Contains("Destination"))
            {
                flightTable.Columns.Add("Destination", typeof(string));
            }
            if (!flightTable.Columns.Contains("Departure Time"))
            {
                flightTable.Columns.Add("Departure Time", typeof(DateTime));
            }
            if (!flightTable.Columns.Contains("Status"))
            {
                flightTable.Columns.Add("Status", typeof(string));
            }
            if (!flightTable.Columns.Contains("Gate"))
            {
                flightTable.Columns.Add("Gate", typeof(string));
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                // Read the contents of the file
                string[] lines = File.ReadAllLines(filePath);
                string[] values;

                for (int i = 0; i < lines.Length; i++)
                {
                    values = lines[i].ToString().Split('/');
                    string[] row = new string[values.Length];

                    for (int j = 0; j < values.Length; j++)
                    {
                        row[j] = values[j].Trim();
                    }

                    flightTable.Rows.Add(row);
                    flights.Add(new Flight(row[0], row[1], DateTime.Parse(row[2])));
                }
            }
            //adding flight to the gridview
            dataGridView1.DataSource = flightTable;
            ManageFlights();
            dataGridView1.Columns[2].DefaultCellStyle.Format = "HH:mm";
        }

        public void InitTimer()
        {
            //timer to update gridview
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000;
            timer.Start();

            System.Windows.Forms.Timer clock = new System.Windows.Forms.Timer();
            clock.Tick += new EventHandler(Clock_Tick);
            clock.Interval = 1000;
            clock.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ManageFlights();
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            //clock
            label1.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        //method for managing flights information
        private void ManageFlights()
        {
            for (int i = 0; i < flightTable.Rows.Count; i++)
            {
                DateTime now = DateTime.Now;
                DateTime flightTime;
                DateTime.TryParse(flightTable.Rows[i][2].ToString(), out flightTime);
                TimeSpan timeUntilDeparture = flightTime.Subtract(now);

                if (timeUntilDeparture < TimeSpan.Zero)
                {
                    if (flightTable.Rows[i][3].ToString() == "Ready-to-take-off")
                    {
                        ReleaseGate(flightTable.Rows[i][4]);
                        //releasing gate
                        gateSemaphore.Release();
                        //log message for showing which flight left the gate
                        invoke(() =>
                        {
                            listBox1.Items.Add(string.Format($"{flightTable.Rows[i][0]} is leaving the gate "));
                        });

                        //getting the runway released flight info
                        Flight flightInfo = new Flight(flightTable.Rows[i][0].ToString(), flightTable.Rows[i][1].ToString(), DateTime.Parse(flightTable.Rows[i][2].ToString()));
                        //initiliazing the service
                        AirportServiceClient service = new AirportServiceClient();
                        //Sending runway released flight information
                        try
                        {
                            FlightService.Flight serviceSide = new FlightService.Flight
                            {
                                FlightCode = flightInfo.FlightCode,
                                Destination = flightInfo.Destination,
                                DepartureTime = flightInfo.DepartureTime
                            };

                            //sending data 
                            bool result = service.InsertFlight(serviceSide);

                            if (result)
                            {
                                Console.WriteLine("Flight information inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                Console.WriteLine("Failed to insert flight information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                        finally
                        {
                            service.Close();
                        }
                        //releasing runway
                        runwaySemaphore.Release();
                        Thread.Sleep(1000);
                        //log message which flight took off
                        invoke(() =>
                        {
                            listBox1.Items.Add(string.Format($"{flightTable.Rows[i][0]} is taking off "));
                        });
                    }
                    //changing status of flight after gate and runway relaesed
                    flightTable.Rows[i][3] = "Take-off";
                    flightTable.Rows[i][4] = "-";
                }
                //if statement for checking status and time of the flight to continue for either wairing for a runway or ready to take off
                else if (timeUntilDeparture >= TimeSpan.Zero && timeUntilDeparture <= TimeSpan.FromMinutes(4) && flightTable.Rows[i][3].ToString() != "Ready-to-take-off")
                {
                    flightTable.Rows[i][3] = "Waiting for A Runway";

                    if (gateSemaphore.WaitOne(0)) // try to acquire a gate
                    {
                        //assigns an available gate to a flight in a flight schedule if the gate is currently set to No gate
                        flightTable.Rows[i][4] = flightTable.Rows[i][4].ToString() == "No gate" ? GetAvailableGate() : flightTable.Rows[i][4];
                        Console.WriteLine(flightTable.Rows[i][3].ToString());
                        //try to acquire runway and checking for waiting for a runway status
                        if (runwaySemaphore.WaitOne(0) && flightTable.Rows[i][3].ToString() == "Waiting for A Runway")
                        {
                            //changing the status
                            flightTable.Rows[i][3] = "Ready-to-take-off";
                            //Log message for displaying which flight changed status to readytotakeof and seconds to take off
                            invoke(() =>
                            {
                                listBox1.Items.Add(string.Format($"{flightTable.Rows[i][0]} is changed status from Runway to Ready-to-take-off, so it is waiting for { Math.Ceiling(timeUntilDeparture.TotalSeconds)} seconds "));
                            });
                        } 
                        gateSemaphore.Release();
                    }
                    //try to acquire runway and checking for waiting for a runway status
                    else if (runwaySemaphore.WaitOne(0) && flightTable.Rows[i][3].ToString() == "Waiting for A Runway")
                    {
                        //changing the status
                        flightTable.Rows[i][3] = "Ready-to-take-off";
                        //log message for ready to take off and show how many second left until take off
                        invoke(() =>
                        {
                            listBox1.Items.Add(string.Format($"{flightTable.Rows[i][0]} is changed status from Runway to Ready-to-take-off, so it is waiting for { Math.Ceiling(timeUntilDeparture.TotalSeconds)} seconds "));
                        });
                    }
                }
                //if statement for checking status and time of the flight to continue for boarding
                else if (timeUntilDeparture > TimeSpan.FromMinutes(4) && timeUntilDeparture <= TimeSpan.FromMinutes(10) && flightTable.Rows[i][3].ToString() != "Boarding")
                {
                    //changing the status
                    flightTable.Rows[i][3] = "Boarding";
                    if (gateSemaphore.WaitOne(0)) // try to acquire a gate
                    {
                        //getting avaliable gate
                        flightTable.Rows[i][4] = GetAvailableGate();
                        //log message for ready to take off and show how many second left until take off
                        invoke(() =>
                        {
                            listBox1.Items.Add(string.Format($"{flightTable.Rows[i][0]} is got the gate {flightTable.Rows[i][4]} for boarding!"));
                        });
                    }
                }
                //if statement for checking status and time of the flight to continue for Waiting for a gate status
                else if (timeUntilDeparture > TimeSpan.FromMinutes(10) && timeUntilDeparture <= TimeSpan.FromMinutes(20) && flightTable.Rows[i][3].ToString() != "Go To Gate")
                {
                    //changing the status
                    flightTable.Rows[i][3] = "Waiting for A Gate";
                    if (gateSemaphore.WaitOne(0)) // try to acquire a gate
                    {
                        //changing the status
                        flightTable.Rows[i][3] = "Go To Gate";
                        //getting avaliable gate
                        flightTable.Rows[i][4] = GetAvailableGate();
                        //log message for showing which flight got which gate
                        invoke(() =>
                        {
                            listBox1.Items.Add(string.Format($"{flightTable.Rows[i][0]} got the gate {flightTable.Rows[i][4]}"));
                        });
                    }
                }
                //if statement for checking status and time of the flight for check in status
                else if (timeUntilDeparture > TimeSpan.FromMinutes(20) && timeUntilDeparture <= TimeSpan.FromHours(3) && flightTable.Rows[i][3].ToString() != "Check In")
                {
                    //changing the status
                    flightTable.Rows[i][3] = "Check In";
                }
            }
        }

        //assigns an available gate to a flight,
        //locking access to the list of gates to prevent concurrent access,
        //and returns the gate number or a string indicating that no gate is available
        private string GetAvailableGate()
        {
            lock (gatesLocker)
            {
                var availableGates = gates.Where(gate => gate.available).ToList();
                if (availableGates.Any())
                {
                    var gateNumber = availableGates[0].number;
                    gates.First(gate => gate.number == gateNumber).available = false;
                    return gateNumber;
                }
                else
                {
                    return "No Gate available";
                }
            }
        }
        //releases the gate with the specified gate number,
        //locking access to the list of gates to prevent concurrent access
        private void ReleaseGate(object gateNumber)
        {
            lock (locker)
            {
                gates.Where(gate => gate.number == gateNumber.ToString()).ToList()[0].available = true;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            //disposes the semaphore object and releases all resources held by it
            gateSemaphore.Dispose();
            runwaySemaphore.Dispose();

        }
        //invoke method to execute the specified function
        void invoke(Action func)
        {
            Invoke(
                new MethodInvoker(
                    () =>
                    {
                        func();
                    }
                )
            );
        }
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //changing the background color of flights based on their status
            if (e.ColumnIndex == 3 && e.Value != null)
            {
                string status = e.Value.ToString();
                if (status == "Ready-to-take-off")
                {
                    e.CellStyle.BackColor = Color.Red;
                }
                else if (status == "Take-off")
                {
                    e.CellStyle.BackColor = Color.Green;
                }
                else if (status == "Boarding")
                {
                    e.CellStyle.BackColor = Color.Yellow;
                }
                else if (status == "Waiting for A Gate")
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                }
                else if (status == "On Time")
                {
                    e.CellStyle.BackColor = Color.LightBlue;
                }
                else if (status == "Waiting for A Runway")
                {
                    e.CellStyle.BackColor = Color.Orange;
                }
                else if (status == "Go To Gate")
                {
                    e.CellStyle.BackColor = Color.Green;
                }
                else if (status == "Check In")
                {
                    e.CellStyle.BackColor = Color.LightGray;
                }
            }
        }
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
        }
    }
}

