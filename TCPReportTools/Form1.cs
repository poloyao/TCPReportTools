using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPReportTools
{
    public partial class Form1 : Form
    {
        //private ObservableCollection<RecvInfo> recvInfos = new ObservableCollection<RecvInfo>();
        private BindingList<RecvInfo> recvInfos = new BindingList<RecvInfo>();

        private BindingList<MessageInfo> messageInfos = new BindingList<MessageInfo>();

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindEnum();
            //bindingSource1.DataSource = recvInfos;
            dataGridView1.DataSource = recvInfos;
            dataGridView2.DataSource = messageInfos;
        }

        public void BindEnum()
        {
            Array arrs = System.Enum.GetValues(typeof(RecvType));
            DataTable dt = new DataTable();
            dt.Columns.Add("String", Type.GetType("System.String"));
            dt.Columns.Add("Value", typeof(int));
            foreach (var arr in arrs)
            {
                DataRow aRow = dt.NewRow();
                aRow[0] = arr.ToString();
                aRow[1] = (int)arr;
                dt.Rows.Add(aRow);
            }
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "String";
            comboBox1.ValueMember = "Value";

            comboBox1.SelectedIndex = 0;
        }

        private void RefreshListData()
        {
            for (int i = 0; i < recvInfos.Count; i++)
            {
                recvInfos[i].Sort = i + 1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((RecvType)((System.Data.DataRowView)comboBox1.SelectedItem)[1] == RecvType.Char && string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Char 类型需要填写长度");
                return;
            }

            int max = 0;
            if (recvInfos.Count > 0)
                max = recvInfos.Max(x => x.Sort);
            max = max == 0 ? 1 : max + 1;

            var tt = (RecvType)((System.Data.DataRowView)comboBox1.SelectedItem)[1];
            recvInfos.Add(new RecvInfo()
            {
                Sort = max,
                Name = textBox2.Text,
                Type = tt,
                Lenght = GetLenght((RecvType)tt),
                Default = string.IsNullOrEmpty(comboBox2.Text) ? tt == RecvType.Char ? "" : "0" : comboBox2.Text
            });
            RefreshListData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var date = dataGridView1.SelectedRows[0];

            textBox1.Text = date.Cells[0].Value.ToString();
            textBox2.Text = date.Cells[1].Value.ToString();
            textBox4.Text = date.Cells[3].Value.ToString();

            var dt = comboBox1.DataSource as DataTable;

            var sing = dt.AsEnumerable().ToList().Single(x => x.ItemArray[0].ToString() == date.Cells[2].Value.ToString());

            comboBox1.SelectedIndex = dt.AsEnumerable().ToList().IndexOf(sing);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var date = dataGridView1.SelectedRows[0].DataBoundItem as RecvInfo;
            recvInfos.Remove(date);
            RefreshListData();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            recvInfos.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var date = dataGridView1.SelectedRows[0].DataBoundItem as RecvInfo;
            int indx = recvInfos.IndexOf(date);
            if (indx > 0)
            {
                recvInfos.Remove(date);
                recvInfos.Insert(indx - 1, date);
                dataGridView1.Rows[indx - 1].Selected = true;
            }
            RefreshListData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var date = dataGridView1.SelectedRows[0].DataBoundItem as RecvInfo;
            int indx = recvInfos.IndexOf(date);
            if (indx < recvInfos.Count - 1)
            {
                recvInfos.Remove(date);
                recvInfos.Insert(indx + 1, date);
                dataGridView1.Rows[indx + 1].Selected = true;
            }
            RefreshListData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //switch ((RecvType)((System.Data.DataRowView)comboBox1.SelectedItem)[1])
            //{
            //    case RecvType.Ushort:
            //        textBox4.Text = 2.ToString();
            //        break;

            //    case RecvType.Int:
            //        textBox4.Text = 4.ToString();
            //        break;

            //    case RecvType.Float:
            //        textBox4.Text = 4.ToString();
            //        break;

            //    case RecvType.Char:
            //        //textBox4.Text = 4.ToString();
            //        break;

            //    default:
            //        break;
            //}
        }

        private int GetLenght(RecvType recvType)
        {
            int result = 0;
            switch (recvType)
            {
                case RecvType.Ushort:
                    result = 2;
                    break;

                case RecvType.Int:
                    result = 4;
                    break;

                case RecvType.Float:
                    result = 4;
                    break;

                case RecvType.Char:
                    result = int.Parse(textBox4.Text);
                    break;

                default:
                    break;
            }

            return result;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<System.Net.Sockets.TcpClient> clients = new List<System.Net.Sockets.TcpClient>();

            System.Net.IPAddress addr = System.Net.IPAddress.Parse(textBox3.Text);
            int port = int.Parse(textBox5.Text);
            int interval = int.Parse(textBox6.Text);

            var server = new SimpleTcpServer().Start(addr, port);

            server.DataReceived += (s, msg) => { };

            server.ClientConnected += (s, client) => { clients.Add(client); };

            server.ClientDisconnected += (s, client) => { clients.Remove(client); };

            Timer timer = new Timer();
            timer.Interval = interval;
            timer.Tick += (s, ea) =>
            {
                var data = SetSendByte();
                foreach (var item in clients)
                {
                    item.Client.Send(data);
                    //DisplaySend(BitConverter.ToString(data));
                    if (messageInfos.Count > 10)
                        messageInfos.RemoveAt(0);
                    messageInfos.Add(new MessageInfo() { Send = true, Massage = BitConverter.ToString(data) });
                }
            };
            timer.Start();
        }

        private void DisplaySend(string msg)
        {
            if (messageInfos.Count > 100)
                messageInfos.RemoveAt(0);
            messageInfos.Add(new MessageInfo() { Send = true, Massage = msg });
            this.Invoke(new Action(() => { dataGridView2.CurrentCell = dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[0]; }));
        }

        /// <summary>
        /// 将16进制的字符串转为byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
                dataGridView1.BeginEdit(true);
        }

        private byte[] SetSendByte()
        {
            List<byte> result = new List<byte>();

            foreach (var item in recvInfos)
            {
                switch (item.Type)
                {
                    case RecvType.Ushort:
                        result.AddRange(BitConverter.GetBytes(ushort.Parse(item.Default)));
                        break;

                    case RecvType.Int:
                        result.AddRange(BitConverter.GetBytes(int.Parse(item.Default)));
                        break;

                    case RecvType.Float:
                        result.AddRange(BitConverter.GetBytes(float.Parse(item.Default)));
                        break;

                    case RecvType.Char:
                        break;

                    default:
                        break;
                }
            }

            return result.ToArray();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            recvInfos.Clear();
            var data = Struct2Info<RecvFurnaceRefreshNotify>();
            data.ForEach(x => { recvInfos.Add(x); });
        }

        private List<RecvInfo> Struct2Info<T>()
        {
            var ssss = (T)Activator.CreateInstance(typeof(T));
            int sort = 1;
            List<RecvInfo> result = new List<RecvInfo>();
            foreach (var item in ssss.GetType().GetFields())
            {
                RecvInfo info = new RecvInfo();
                info.Sort = sort++;
                info.Name = item.Name;
                switch (item.FieldType.Name)
                {
                    case "UInt16":
                        info.Type = RecvType.Ushort;
                        break;

                    case "Single":
                        info.Type = RecvType.Float;
                        break;

                    case "Int32":
                        info.Type = RecvType.Int;
                        break;

                    default:
                        break;
                }
                info.Lenght = GetLenght(info.Type);
                info.Default = info.Type == RecvType.Char ? "" : "0";
                result.Add(info);
            }
            return result;
        }
    }

    public class RecvInfo
    {
        public int Sort { get; set; }
        public string Name { get; set; }
        public RecvType Type { get; set; }
        public int Lenght { get; set; }
        public string Default { get; set; }
    }

    public enum RecvType
    {
        Ushort = 0,
        Int = 1,
        Float = 2,
        Char = 3,
    }

    public class MessageInfo
    {
        public bool Send { get; set; }
        public DateTime UDP { get; set; } = DateTime.Now;
        public string Massage { get; set; }
    }
}