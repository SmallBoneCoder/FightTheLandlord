using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer_FightTheLandlord
{
    public class Server
    {
        private Socket _serverSocket;//socket对象
        private IPEndPoint _point;//网络节点
        private int _listenCount = 10;//同时监听等待人数
        private bool isRunning = true;
        private const int _bufferLength = 16* 16;//缓存大小
        //private byte[] _buffer;//缓存区
        private Dictionary<string, Socket> _allClients;//所有连接的客户端
        private List<string> _allNames;//所有连接的客户端的名字
        private Dictionary<string, byte[]> _allBuffers;//所有客户端的接收缓冲区
        private Dictionary<string, MyParser> _allParser;//所有客户端的解析类

        private List<string> _toBeRemoveNames;//要删除的客户端名字
        //private List<Thread> _threadPool;//线程池
        //private Dictionary<string, ConcurrentQueue<string>> _allSendMessage;
        public Server(string ip, string port)
        {

            InitServer(ip, port);
        }


        public void InitServer(string ipaddr, string port)
        {
            //转换IP地址
            IPAddress ipaddress = IPAddress.Parse(ipaddr);
            //转换端口号
            _point = new IPEndPoint(ipaddress, int.Parse(port));
            //创建监听的Socket
            _serverSocket = new Socket(AddressFamily.InterNetwork, //IPV4地址
                SocketType.Stream, //流式连接（长连接）
                ProtocolType.Tcp //TCP传输协议
                );
            //初始化客户端字典
            _allClients = new Dictionary<string, Socket>();
            _allNames = new List<string>();
            _allBuffers = new Dictionary<string, byte[]>();
            _allParser = new Dictionary<string, MyParser>();
            _toBeRemoveNames = new List<string>();

        }
        /// <summary>
        /// 开始监听
        /// </summary>
        public void StartListen()
        {
            if (!isRunning) return;
            try
            {
                //绑定网络结点
                _serverSocket.Bind(_point);
                //监听队列长度（等待人数）
                _serverSocket.Listen(_listenCount);
                //开始监听
                Console.WriteLine("服务器开始监听");
                _serverSocket.BeginAccept(new AsyncCallback(CallBackAccept), _serverSocket);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void CallBackAccept(IAsyncResult result)
        {
            if (!isRunning) return;
            Socket server = (Socket)result.AsyncState;
            Socket t_socket = null;
            //接收一个连接
            try
            {
                t_socket = server.EndAccept(result);
                //获取客户端信息
                string ipname = t_socket.RemoteEndPoint.ToString();
                //消息提示
                Console.WriteLine(ipname);
                //添加进客户端列表
                if (!_allNames.Contains(ipname))
                {
                    _allNames.Add(ipname);
                    _allClients.Add(ipname, t_socket);
                    _allBuffers.Add(ipname, new byte[_bufferLength]);
                    _allParser.Add(ipname, new MyParser());
                }
                HandleAccepted(t_socket);
                HandleAccepted(ipname);
                //开始接收数据
                t_socket.BeginReceive(
                    _allBuffers[ipname],
                    0,
                    _bufferLength,
                    SocketFlags.None,
                    new AsyncCallback(RecieveMsg), t_socket);
                //开始接收下一个连接
                server.BeginAccept(new AsyncCallback(CallBackAccept), result.AsyncState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public virtual void HandleAccepted(Socket client)
        {

        }
        public virtual void HandleAccepted(string ipName)
        {

        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="client_socket"></param>
        private void RecieveMsg(IAsyncResult client_socket)
        {
            if (!isRunning) return;
            Socket client = (Socket)client_socket.AsyncState;
            MyParser parser = _allParser[client.RemoteEndPoint.ToString()];
            //byte[] _buffer = new byte[_bufferLength];
            try
            {
                //接收字节数据并返回长度
                int n = client.EndReceive(client_socket);
                //Console.WriteLine(BitConverter.ToString(_buffer));
                //if (n == 0)
                //{
                //    return;//断开连接
                //}
                parser.ConstructPacket(_allBuffers[client.RemoteEndPoint.ToString()], n);
                byte[] temp = null;
                //数据处理
                while ((temp = parser.GetSingleData()) != null)
                {
                    //将消息转化为字符串
                    string msg = parser.DecoderData(temp);
                    //Console.WriteLine(msg);
                    HandleRecievedData(client.RemoteEndPoint.ToString(), msg);
                    HandleRecievedData(client.RemoteEndPoint.ToString(),temp);
                }
                //该用户已经离线
                if (!_allNames.Contains(client.RemoteEndPoint.ToString()))
                {
                    Console.WriteLine("玩家(" + client.RemoteEndPoint.ToString() + ")离线");
                    
                    return;
                }
                client.BeginReceive(
                _allBuffers[client.RemoteEndPoint.ToString()],
                0,
                _bufferLength,
                SocketFlags.None,
                new AsyncCallback(RecieveMsg), client);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("玩家(" + client.RemoteEndPoint.ToString() + ")离线");
                Disconnected(client.RemoteEndPoint.ToString());
                return;
            }

        }
        public virtual void HandleRecievedData(string ipname,string data)
        {

        }
        public virtual void HandleRecievedData(string ipname,byte[] data)
        {

        }
        private void Disconnected(string ipname)
        {
            HandleDisconnected(ipname);
            _allClients[ipname].Close();
            _allNames.Remove(ipname);
            _allClients.Remove(ipname);
            _allBuffers.Remove(ipname);
            _allParser.Remove(ipname);
            Console.WriteLine(GC.GetTotalMemory(false));
            GC.Collect();
            Console.WriteLine(GC.GetTotalMemory(false));
        }
        public virtual void HandleDisconnected(string ipname)
        {

        }
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="shieldName">屏蔽的IP</param>
        /// <param name="msg">字节消息</param>
        public void BroadcastMsg(string shieldName, byte[] msg)
        {
            //if(ipname!=null)_allClients[ipname].Send(parser.EncoderData((ushort)1, msg));
            for (int i = 0; i < _allNames.Count; i++)
            {
                if (_allNames.Count - 1 < i) continue;
                //将消息转发给其他用户
                if (_allNames[i] != shieldName)
                {
                    //_allClients[_allNames[i]].Send(msg);
                    SendDataBegin(_allNames[i], msg);
                }
            }

            RemoveAllClients();//转发完后移除所有客户端
        }
        /// <summary>
        /// 指定转发
        /// </summary>
        /// <param name="IPNames">转发的IP列表</param>
        /// <param name="msg">消息</param>
        /// <param name="shieldName">屏蔽IP</param>
        public void TransmitMsg(string[] IPNames, byte[] msg, string shieldName)
        {
            for (int i = 0; i < IPNames.Length; i++)
            {
                //将消息转发给除屏蔽之外的客户端
                if (IPNames[i] != shieldName)
                {
                    SendDataBegin(IPNames[i], msg);
                }
            }
            RemoveAllClients();//转发完后移除所有客户端
        }
        private void RemoveAllClients()
        {
            //Console.WriteLine("正在移除客户端");
            foreach (string ipname in _toBeRemoveNames)
            {
                Console.WriteLine("玩家(" + ipname + ")离线");
                Disconnected(ipname);
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ipname">客户端ip</param>
        /// <param name="msg">消息</param>
        public void SendDataBegin(string ipname, byte[] msg)
        {
            try
            {
                _allClients[ipname].BeginSend(
                          msg,
                          0,
                          msg.Length,
                          SocketFlags.None,
                          new AsyncCallback(SendDataEnd),
                          _allClients[ipname]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //Console.WriteLine("玩家("+ipname+")离线");
                _toBeRemoveNames.Add(ipname);//添加进移除名单
            }

        }
        public virtual void SendDataEnd(IAsyncResult ar)
        {
            ((Socket)ar.AsyncState).EndSend(ar);
        }

        public void CloseServer()
        {
            if (!isRunning) return;
            isRunning = false;
            try
            {
                //先关闭子连接
                for (int i = 0; i < _allNames.Count; i++)
                {
                    _allClients[_allNames[i]].Close();
                }
                //关闭主连接
                _serverSocket.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
