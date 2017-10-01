using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace TestSerialPort
{
    public partial class Sub1GHz : Form
    {
        public Sub1GHz()
        {
            InitializeComponent();      
        }

        UInt16 availableDevices = 1;

        RFDevice device1 = new RFDevice();
        RFDevice device2 = new RFDevice();
        RFDevice device3 = new RFDevice();
        RFDevice device4 = new RFDevice();
        RFDevice device5 = new RFDevice();

        SerialPort comPort = new SerialPort();

        private delegate void SetTextDeleg(string text);
        
        private void button1_Click(object sender, EventArgs e)
        {

            if (button1.Text == "Open Port")
            {
                button1.Text = "Close Port";
                comPort.PortName = Convert.ToString(cbPortNames.Text);
                comPort.BaudRate = Convert.ToInt32(cbBaudRate.Text);
                comPort.DataBits = Convert.ToInt16(cbDataBits.Text);
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbStopBits.Text);
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), cbParity.Text);
                comPort.Handshake = Handshake.None;
                comPort.Encoding = Encoding.GetEncoding(1252);

                try
                {
                    if (!(comPort.IsOpen))
                        comPort.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening/writing to serial port :: "
                                    + ex.Message, "Error!");
                    comPort.Close();
                }    
            }
            else if (button1.Text == "Close Port")
            {
                button1.Text = "Open Port";
                button3.Enabled = true;
                comPort.Close();

                cbPortNames.Text = "";
                cbBaudRate.Text = "";
                cbDataBits.Text = "";
                cbStopBits.Text = "";
                cbParity.Text = "";
                cbNodeAddresses.Text = "";

                cbPortNames.Items.Clear();
                cbBaudRate.Items.Clear();
                cbDataBits.Items.Clear();
                cbStopBits.Items.Clear();
                cbParity.Items.Clear();
                cbNodeAddresses.Items.Clear();
            }

            comPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);        
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            string data = comPort.ReadLine();
            BeginInvoke(new SetTextDeleg(si_DataReceived),
                             new object[] { data });   
        }

        private void si_DataReceived(string data)
        {     
            textBox1.Text = "Packet received. Sequence number: " + Convert.ToInt32(data[1]); 

            chart1.Series["RSSI"].Points.AddY(Convert.ToInt16((sbyte)data[2]));

            double batteryVoltage = (Convert.ToDouble(Convert.ToInt32(data[0]))) * 0.02 + 0.6;
            label7.Text = Convert.ToString(batteryVoltage) + "V";
            connectedDevices.Text = Convert.ToString(availableDevices);

           /* switch (data[0])
            {
                case '1':
                    device1.NodeAddress = data[0];
                    device1.AverageRSSI = data[1];
                    device1.Status = data[2];
                    device1.PacketLength = data[3];
                    device1.SymbolRate = data[4];
                    device1.TxPower = data[5];
                    device1.UpdateTime = DateTime.Now.ToString("HH:mm:ss");
                    break;
                case '2':
                    device2.NodeAddress = data[0];
                    device2.AverageRSSI = data[1];
                    device2.Status = data[2];
                    device2.PacketLength = data[3];
                    device2.SymbolRate = data[4];
                    device2.TxPower = data[5];
                    device2.UpdateTime = DateTime.Now.ToString("HH:mm:ss");
                    break;
                case '3':
                    device3.NodeAddress = data[0];
                    device3.AverageRSSI = data[1];
                    device3.Status = data[2];
                    device3.PacketLength = data[3];
                    device3.SymbolRate = data[4];
                    device3.TxPower = data[5];
                    device3.UpdateTime = DateTime.Now.ToString("HH:mm:ss");
                    break;
                case '4':
                    device4.NodeAddress = data[0];
                    device4.AverageRSSI = data[1];
                    device4.Status = data[2];
                    device4.PacketLength = data[3];
                    device4.SymbolRate = data[4];
                    device4.TxPower = data[5];
                    device4.UpdateTime = DateTime.Now.ToString("HH:mm:ss");
                    break;
                case '5':
                    device5.NodeAddress = data[0];
                    device5.AverageRSSI = data[1];
                    device5.Status = data[2];
                    device5.PacketLength = data[3];
                    device5.SymbolRate = data[4];
                    device5.TxPower = data[5];
                    device5.UpdateTime = DateTime.Now.ToString("HH:mm:ss");
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            switch (cbNodeAddresses.Text)
            {
                case "0x01":
                    lbAverageRSSI.Text = device1.AverageRSSI + " dBm";
                    lbStatus.Text = device1.Status;
                    lbPacketLength.Text = device1.PacketLength + " bytes";
                    lbSymbolRate.Text = device1.SymbolRate + " kBaud";
                    lbTxPower.Text = device1.TxPower + " dBm";
                    break;
                case "0x02":
                    lbAverageRSSI.Text = device2.AverageRSSI + " dBm";
                    lbStatus.Text = device2.Status;
                    lbPacketLength.Text = device2.PacketLength + " bytes";
                    lbSymbolRate.Text = device2.SymbolRate + " kBaud";
                    lbTxPower.Text = device2.TxPower + " dBm";
                    break;
                case "0x03":
                    lbAverageRSSI.Text = device3.AverageRSSI + " dBm";
                    lbStatus.Text = device3.Status;
                    lbPacketLength.Text = device3.PacketLength + " bytes";
                    lbSymbolRate.Text = device3.SymbolRate + " kBaud";
                    lbTxPower.Text = device3.TxPower + " dBm";
                    break;
                case "0x04":
                    lbAverageRSSI.Text = device4.AverageRSSI + " dBm";
                    lbStatus.Text = device4.Status;
                    lbPacketLength.Text = device4.PacketLength + " bytes";
                    lbSymbolRate.Text = device4.SymbolRate + " kBaud";
                    lbTxPower.Text = device4.TxPower + " dBm";
                    break;
                case "0x05":
                    lbAverageRSSI.Text = device5.AverageRSSI + " dBm";
                    lbStatus.Text = device5.Status;
                    lbPacketLength.Text = device5.PacketLength + " bytes";
                    lbSymbolRate.Text = device5.SymbolRate + " kBaud";
                    lbTxPower.Text = device5.TxPower + " dBm";
                    break;
                default:       
                    break;
            }*/
            

            if (cbNodeAddresses.Text == "0x01")
            {
                lbAverageRSSI.Text = Convert.ToString(Convert.ToInt16((sbyte)data[2])) + " dBm";
                lbStatus.Text = "OK";
                lbPacketLength.Text = "31 bytes";
                lbSymbolRate.Text = "50 kBaud";
                lbTxPower.Text = "14 dBm";
            }
            else {
                lbAverageRSSI.Text = "0 dBm";
                lbStatus.Text = "NOT OK";
                lbPacketLength.Text = "0 bytes";
                lbSymbolRate.Text = "0 kBaud";
                lbTxPower.Text = "0 dBm";
            }   
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] ArrayOfComPorts = null;
            string[] ArrayOfBaudRate = { "300", "600", "1200", "2400", "9600", "14400",
                                          "19200", "38400", "57600", "115200" };
            string[] ArrayOfDataBits = { "5", "6", "7", "8" };
            string[] ArrayOfStopBits = { "1", "1.5", "2" };
            string[] ArrayOfParity = { "None", "Odd", "Even", "Mark", "Space" };
            string[] ArrayOfNodeAddresses = { "0x01", "0x02", "0x03", "0x04", "0x05" };

            ArrayOfComPorts = SerialPort.GetPortNames();

            cbPortNames.Items.AddRange(ArrayOfComPorts);
            cbPortNames.Text = ArrayOfComPorts[0];

            cbBaudRate.Items.AddRange(ArrayOfBaudRate);
            cbBaudRate.Text = cbBaudRate.Items[4].ToString();

            cbDataBits.Items.AddRange(ArrayOfDataBits);
            cbDataBits.Text = cbDataBits.Items[3].ToString();

            cbStopBits.Items.AddRange(ArrayOfStopBits);
            cbStopBits.Text = cbStopBits.Items[0].ToString();

            cbParity.Items.AddRange(ArrayOfParity);
            cbParity.Text = cbParity.Items[0].ToString();

            cbNodeAddresses.Items.AddRange(ArrayOfNodeAddresses);
            cbNodeAddresses.Text = cbNodeAddresses.Items[0].ToString();

            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            chart1.Series["RSSI"].Points.Clear();
        }

        private void Sub1GHz_FormClosing(object sender, FormClosingEventArgs e)
        {
            comPort.Close();
        }
    }

    public class RFDevice {

        public string NodeAddress;
        public string AverageRSSI;
        public string Status;
        public string PacketLength;
        public string SymbolRate;
        public string TxPower;
        public string UpdateTime;

    }
}
