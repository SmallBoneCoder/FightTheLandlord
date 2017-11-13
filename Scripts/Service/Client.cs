using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace TestClient
{
    //public delegate void TextHandle(string s);
    public class Client
    {
        public bool ConnectFail = false;//连接失败
        private Socket _clientSocket;//客户端套接字
        private IPEndPoint _point;//网络节点
        private const int _bufferLength = 1024;//缓存大小
        private byte[] _buffer;//缓存区
        private Thread _thread;
        public Queue<string> MessageBox;
        //public TextHandle txtHandle;
        private bool isThreadEnd = false;//是否终止线程
        
        public bool IsConnected { get
            {
                return _clientSocket.Connected;
            }
        }

        public Client(string serverip, string port)
        {
            InitClient(serverip, port);
        }
        public string GetIPName()
        {
            return _clientSocket.LocalEndPoint.ToString();
        }
        /// <summary>
        /// 初始化客户端
        /// </summary>
        /// <param name="ipaddr">ip地址</param>
        /// <param name="port">端口</param>
        public void InitClient(string ipaddr, string port)
        {
            if (_clientSocket!=null&&_clientSocket.Connected)
            {
                _clientSocket.Close();
            }
            //转换IP地址
            IPAddress ipaddress = IPAddress.Parse(ipaddr);
            //转换端口号
            _point = new IPEndPoint(ipaddress, int.Parse(port));
            //创建监听的Socket
            _clientSocket = new Socket(AddressFamily.InterNetwork, //IPV4地址
                SocketType.Stream, //流式连接（长连接）
                ProtocolType.Tcp //TCP传输协议
                );
            //初始化缓存区
            _buffer = new byte[_bufferLength];
            MessageBox = new Queue<string>();
            GC.Collect();
        }
        /// <summary>
        /// 开始连接
        /// </summary>
        public void StartConnect()
        {
            Thread start = new Thread(() =>
              {
                  try
                  {
                      //开始连接
                      _clientSocket.Connect(_point);
                      //显示消息
                      Debug.Log("连接成功");
                      Debug.Log("服务器:" + _clientSocket.RemoteEndPoint.ToString());
                      //开启线程监听消息
                      ConnectFail = false;
                      _thread = new Thread(RecieveMsg);
                      _thread.IsBackground = true;
                      _thread.Start();
                  }
                  catch (Exception e)
                  {
                      //Console.WriteLine(e.ToString());
                      ConnectFail = true;
                      Debug.Log(e.ToString());
                  }
              });
            start.IsBackground = true;
            start.Start();
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        public void RecieveMsg()
        {
            while (!isThreadEnd)
            {
                try
                {
                    //接收字节数据并返回长度
                    int n = _clientSocket.Receive(_buffer);
                    //Debug.Log(n);
                    MyParser.ConstructPacket(_buffer, n);
                    byte[] temp = null;
                    while ((temp = MyParser.GetSingleData()) != null)
                    {
                        //将消息转化为字符串
                        string msg = MyParser.DecoderData(temp);
                        //显示消息
                        //Console.WriteLine(msg);
                        Debug.Log(msg);
                        lock (MessageBox)
                        {
                            MessageBox.Enqueue(msg);
                        }
                        //txtHandle(msg);
                    }
                }
                catch(Exception e)
                {
                    //Console.WriteLine(e.ToString());
                    Debug.Log(e.ToString());
                    break;//断开连接
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息</param>
        public void SendMsg(string msg)
        {
            try
            {
                //_clientSocket.Send(MyParser.EncoderData(msg));
                byte[] buffer = MyParser.EncoderData(msg);
                _clientSocket.BeginSend(buffer,0,buffer.Length,
                    SocketFlags.None,
                    (ia) => { ((Socket)ia.AsyncState).EndSend(ia); },
                    _clientSocket
                    );
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
                Debug.Log(e.ToString());
                StartConnect();
            }

        }
        public void CloseClient()
        {
            //_thread.Interrupt();
            isThreadEnd = true;
            //_clientSocket.Disconnect(false);
            if(_clientSocket.Connected)_clientSocket.Close();
            

        }
    }

    class MyParser
    {
        //private const string _head = "HH";
        //private const string _tail = "TT";

        private static Queue<List<byte>> _packets = new Queue<List<byte>>();
        private static List<byte> _currentPacket = new List<byte>();
        //private static byte[] _cmdType_byte;
        private static byte[] _cmdContent_byte;
        private static byte[] _head_byte;
        //private byte[] _tail_byte;
        private void initProtocol()
        {


        }

        /// <summary>
        /// 将消息转化为字节数组
        /// </summary>
        /// <param name="cmdType">消息类型</param>
        /// <param name="cmdContent">消息内容</param>
        /// <returns></returns>
        public static byte[] EncoderData( string cmdContent)
        {
            //_cmdType_byte = BitConverter.GetBytes(cmdType);
            _cmdContent_byte = Encoding.UTF8.GetBytes(cmdContent);
            //获取全部指令长度
            int len = _cmdContent_byte.Length; //_cmdType_byte.Length;
            //将长度作为数据头转化为字节数组（int32—四个字节）
            _head_byte = BitConverter.GetBytes(len);
            //创建一个大小为头+内容大小的字节数组
            byte[] buffer = new byte[
                _head_byte.Length + len];
            //将头字节放入buffer
            _head_byte.CopyTo(buffer, 0);
            //将指令类型字节放入buffer
            //_cmdType_byte.CopyTo(buffer, _head_byte.Length);
            //将指令内容字节放入buffer
            //_cmdContent_byte.CopyTo(buffer, _cmdType_byte.Length + _head_byte.Length);
            _cmdContent_byte.CopyTo(buffer, _head_byte.Length);
            return buffer;
        }
        /// <summary>
        /// 将字节数组转化为消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>字符串首位为消息类型</returns>
        public static string DecoderData(byte[] buffer)
        {
            string msg = "";
            //msg += BitConverter.ToInt32(buffer, 0);
            //msg += BitConverter.ToUInt16(buffer, 4);
            msg += Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
            return msg;
        }
        static int _restLength = 0;
        static int _pointer = 0;
        static int _restHead = 4;
        static bool isBodyFinish = true;
        public static void ConstructPacket(byte[] buffer, int length)
        {
            while (true)
            {
                //身体读完了
                if (isBodyFinish)
                {
                    //读头
                    for (int i = 0; i < _restHead; i++)
                    {
                        //头没读完
                        if (_pointer >= length)
                        {
                            //isHeadFinish = false;
                            _restHead = 4 - i;
                            _pointer = 0;
                            return;
                        }
                        _currentPacket.Add(buffer[_pointer++]);
                    }
                    _restHead = 4;
                }
                int len = 0;
                if (_restLength != 0)
                {
                    len = _restLength;
                }
                else
                {
                    len = BitConverter.ToInt32(_currentPacket.ToArray(), 0);
                }
                //读身体数据
                for (int i = 0; i < len; i++)
                {
                    if (_pointer >= length)
                    {
                        isBodyFinish = false;
                        _restLength = len - i;
                        _pointer = 0;
                        return;
                    }
                    _currentPacket.Add(buffer[_pointer++]);
                }
                //将当前数据包添加到队列
                _packets.Enqueue(_currentPacket);
                _currentPacket = new List<byte>();//重置包
                isBodyFinish = true;
                _restLength = 0;
                //顺利读到末尾
                if (_pointer == length)
                {
                    _pointer = 0;

                    return;
                }
            }

        }
        public static byte[] GetSingleData()
        {
            if (_packets.Count <= 0) return null;
            byte[] single = _packets.Dequeue().ToArray();

            return single;
        }
    }
}
