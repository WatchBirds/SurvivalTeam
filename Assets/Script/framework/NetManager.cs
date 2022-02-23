using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{ 
      //事件
      public enum NetEvent
      {
            ConnectSucc = 1,
            ConnectFail = 2,
            Close =3
      }
      //事件委托类型
      public delegate void EventListener(string err);
      //消息委托类型
      public delegate void MsgListener(MsgBase msgBase);
      //事件监听列表
      private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();
      //消息监听列表
      private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
      //定义套接字
      private static Socket socket;
      //接收缓冲区
      private static ByteArry readBuff;
      //写入队列
      private static Queue<ByteArry> writeQueue;
      //是否正在连接
     private static bool isConnecting =false;
      //是否正在关闭
      private static bool isClosing = false;
      //消息列表
      private static List<MsgBase> msgList = new List<MsgBase>();
      //消息列表长度
      private static int msgCount = 0;
      //每一次Update处理的消息量
      readonly static int MAX_MESSAGE_FIRE = 10;
      //是否启用心跳机制
      public static bool isUsingPing = true;
      //心跳协议发送间隔时间
      public static int pingInterval = 30;
      //上一次发送PING协议的时间
      private static float lastPingTime = 0;
      //上一次收到PONG协议的时间
      private static float lastPongTime = 0;

      //初始化状态
      private static void InitState()
      {
            //Socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //接收缓冲区
            readBuff = new ByteArry();
            //写入队列
            writeQueue = new Queue<ByteArry>();
            //是否正在连接
            isConnecting = false;
            //是否正在关闭
            isClosing = false;
            //消息列表
            msgList = new List<MsgBase>();
            //消息列表长度
            msgCount = 0;
            //上一次发送PING协议的时间
            lastPingTime = Time.time; ;
            //上一次收到PONG协议的时间
            lastPongTime = Time.time;
            //监听PONG协议
            if(!msgListeners.ContainsKey("MsgPong"))
            {
                  AddMsgListener("MsgPong", OnMsgPong);
            }
}
      private static void OnMsgPong(MsgBase msgBase)
      {
            lastPongTime = Time.time;
      }
      #region Socket: Connect Receive Send Close
      /// <summary>
      /// 连接
      /// </summary>
      /// <param name="ip">IP地址</param>
      /// <param name="port">端口号</param>
      public static void Connect(string ip,int port)
      {
            //状态判断
            if(socket!=null && socket.Connected)
            {
                  Debug.Log("Connect fail: already connected !");
                  return;
            }
            if (isConnecting)
            {
                  Debug.Log("Connect fail: isConnecting");
                  return;
            }
            //初始化
            InitState();
            //参数设置
            socket.NoDelay = true;//取消Nagle算法
            //Connect
            isConnecting = true;
            socket.BeginConnect(ip, port,ConnectCallback, socket);
      }
      private static void ConnectCallback(IAsyncResult ar)
      {
            try
            {
                  Socket socket = (Socket)ar.AsyncState;
                  socket.EndConnect(ar);
                  Debug.Log("Socket Connect Succ");
                  FireEvent(NetEvent.ConnectSucc, "");
                  isConnecting = false;
                  //Receive
                  socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
            }
            catch(SocketException ex)
            {
                  Debug.Log("Socket Connect fail: " + ex.Message);
                  FireEvent(NetEvent.ConnectFail, ex.Message);
                  isConnecting = false;
            }
      }
      //接收回调
      private static void ReceiveCallback(IAsyncResult ar)
      {
            try
            {
                  Socket socket = (Socket)ar.AsyncState;
                  //获取数据长度
                  int count = socket.EndReceive(ar);
                  if (count == 0)
                  {
                        FireEvent(NetEvent.Close, "断开连接");
                        Close();
                        return;
                  }
                  readBuff.writeIdx += count;
                  //处理二进制消息
                  OnReceiveData();
                  //继续接收消息
                  if(readBuff.remain<8)
                  {
                        readBuff.MoveBytes();
                        readBuff.ReSize(readBuff.length * 2);
                  }
                  socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
            }
            catch(SocketException ex)
            {
                  FireEvent(NetEvent.Close, "服务器关闭");
                  Debug.Log("Socket Receive fail: " + ex.Message);
            }
      }
      //接收消息处理
      public static void OnReceiveData()
      {
            //消息长度过小
            if(readBuff.length<=2)
            { return; }
            //获取消息体长度
            int readIdx = readBuff.readIdx;
            byte[] bytes = readBuff.bytes;
            Int16 bodyLength = (Int16)(bytes[readIdx + 1] << 8 | bytes[readIdx]);
            //消息接收不完整
            if (readBuff.length < bodyLength+2)
            {
                  return;
            }
            readBuff.readIdx += 2;
            //解析协议名 
            int nameCount = 0;
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
            
            if(protoName == "")
            {
                  Debug.Log("OnReceiveData MsgBase.DecodeName fail");
                  return;
            }
            readBuff.readIdx += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMove();
            //添加到队列消息
            lock(msgList)
            {
                  msgList.Add(msgBase);
            }
            msgCount++;
            if(readBuff.length>2)
            {
                  OnReceiveData();
            }
      }
      /// <summary>
      /// 发送数据
      /// </summary>
      /// <param name="msg">协议</param>
      public static void Send(MsgBase msg)
      {
            //状态判断
            if(socket == null||!socket.Connected)
            { return; }
            if(isConnecting)
            { return; }
            if(isClosing)
            { return; }
            //数据编码
            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);
            int len = nameBytes.Length + bodyBytes.Length;
            byte[] sendBytes = new byte[2 + len];
            //组装长度
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);
            //组装名称内容
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            //组装消息体
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);//组装结果内容顺序： 消息长度（不包含自身二个字节长度） 协议名长度 协议名 消息体
            //写入队列
            ByteArry ba = new ByteArry(sendBytes);
            int count = 0;
            lock (writeQueue)//解决线程冲突
            {
                  writeQueue.Enqueue(ba);
                  count = writeQueue.Count;
            }
            if (count == 1)
            {
                  socket.BeginSend(ba.bytes, 0, ba.length, 0, SendCallback, socket);
            }
      }
      //Send回调 
      private static void SendCallback(IAsyncResult ar)
      {
            Socket socket = (Socket)ar.AsyncState;
            //状态判断
            if (socket == null||!socket.Connected)
            { return; }
            //EndSend
            int count = socket.EndSend(ar);
            //获取写入队列第一条数据
            ByteArry ba;
            lock(writeQueue)
            {
                  ba = writeQueue.First();
            }
            //完整发送
            ba.readIdx += count;
            if (ba.length == 0)
            {
                  lock (writeQueue)
                  {
                        writeQueue.Dequeue();
                        if (writeQueue.Count != 0)
                        {
                              ba = writeQueue.First();
                        }
                        else
                        {
                              ba = null;
                        }
                  }
            }
            //如果发送不完整或者发送完整且还有数据且不为空继续发送
            if(ba!=null)
            {
                  socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
            }
            else if(isClosing)//发完且处于正在关闭状态
            { socket.Close(); FireEvent(NetEvent.Close, "连接服务器失败"); }
      } 
      /// <summary>
      /// 断开连接
      /// </summary>
      public static void Close()
      {
            //状态判断
            if(socket == null||!socket.Connected)
            { return; }
            if (isClosing)
            { return; }
            //如果还有数据在发送
            if(writeQueue.Count>0)
            {
                  isClosing = true;
            }
            //没有数据需要发送
            else
            {
                  socket.Close();
            }
      }
      #endregion
      /// <summary>
      /// 添加监听事件
      /// </summary>
      /// <param name="netEvent">事件类型</param>
      /// <param name="listener">监听函数</param>
      public static void AddEventListener(NetEvent netEvent,EventListener listener)
      {
            //添加事件
            if(eventListeners.ContainsKey(netEvent))
            {
                  eventListeners[netEvent] += listener;
            }
            else
            {
                  eventListeners.Add(netEvent, listener);
            }  
      }
      /// <summary>
      /// 删除监听事件
      /// </summary>
      /// <param name="netEvent">事件类型</param>
      /// <param name="listener">监听函数</param>
      public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
      {
            if (eventListeners.ContainsKey(netEvent))
            {
                  eventListeners[netEvent] -= listener;
                  if (eventListeners[netEvent] == null)
                  { eventListeners.Remove(netEvent); }
            }
      }
      //分发事件
      private static void FireEvent(NetEvent netEvent,string str)
      {
            if (eventListeners.ContainsKey(netEvent))
            {
                  eventListeners[netEvent](str);
            }
      }
      /// <summary>
      /// 添加消息监听
      /// </summary>
      /// <param name="msgName">消息名称</param>
      /// <param name="listener">消息</param>
      public static void AddMsgListener(string msgName,MsgListener listener)
      {
            //添加
            if(msgListeners.ContainsKey(msgName))
            {
                  msgListeners[msgName] += listener;
            }
            else
            {
                  msgListeners[msgName] = listener;
            }
      }
      /// <summary>
      /// 删除消息监听
      /// </summary>
      /// <param name="msgName">消息名称</param>
      /// <param name="listener">消息</param>
      public static void RemoveMsgListener(string msgName,MsgListener listener)
      {
            if (msgListeners.ContainsKey(msgName))
            {
                  msgListeners[msgName] -= listener;
                  if (msgListeners[msgName] == null)
                  {
                        msgListeners.Remove(msgName);
                  }
            }
      }
      //分发消息
      private static void FireMsg(string msgName,MsgBase msgBase)
      {
            if (msgListeners.ContainsKey(msgName))
            {
                  
                  msgListeners[msgName](msgBase);
            }
      }

      public static void Update()
      {
            MsgUpdate();
            PingUpdate();
      }
      //更新收到的消息
      private static void MsgUpdate()
      {
            if (msgCount == 0)
            { return; }
            //重复处理消息
            for (int i = 0; i<MAX_MESSAGE_FIRE;i++)
            {
                  //获取第一条消息
                  MsgBase msgBase = null;
                  lock (msgList)
                  {
                        if (msgList.Count > 0)
                        {
                              msgBase = msgList[0];
                              msgList.RemoveAt(0);
                              msgCount--;
                        }
                  }    
                  //分发消息
                  if (msgBase != null)
                  {
                        Debug.Log("Receive: " + msgBase.protoName);
                        FireMsg(msgBase.protoName, msgBase);
                  }
                  else
                  { break; }
            }
      }
      private static void PingUpdate()
      {
            //是否启用
            if (!isUsingPing)
            { return; }
            //发送PING
            if (Time.time-lastPingTime>pingInterval)
            {
                  MsgPing msgPing = new MsgPing();
                  Send(msgPing);
                  lastPingTime = Time.time;
            }
            //检查PONG时间
            if (Time.time-lastPongTime>pingInterval*4)
            {
                  Close();
            }
      }
}
