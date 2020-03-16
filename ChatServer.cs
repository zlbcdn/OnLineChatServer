using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;
using Newtonsoft.Json;

namespace OnChatServer
{
    class ChatServer
    {
        static void Main(string[] args)
        {
 
                IList<SocketClient> socketList = new List<SocketClient>();
                var socketServer = new WebSocketServer("ws://10.37.24.14:7181");
                socketServer.RestartAfterListenError = true;

                socketServer.Start(socket => {
                    //保存socket
                    socket.OnOpen = () => {
                        Console.WriteLine("创建链接");
                    };

                    //去除socket
                    socket.OnClose = () => {

                        var tempList = (from r in socketList where r.TheSocketClient == socket select r).ToList<SocketClient>();
                        if (tempList.Count == 1)
                        {
                            #region 告知客户端下线
                            var end_message_for_client = (from r in socketList where r.SocketForm == tempList[0].SocketTo select r).ToList<SocketClient>();
                            if (end_message_for_client.Count == 1)
                            {

                                MessageDataModel end_message_model = new MessageDataModel();
                                end_message_model.message_type = "close";
                                end_message_model.message_from = tempList[0].SocketForm;
                                end_message_model.message_to = tempList[0].SocketTo;
                                end_message_model.message_content = "";

                                string end_str = JsonConvert.SerializeObject(end_message_model);

                                end_message_for_client[0].TheSocketClient.Send(end_str);
                            }
                            #endregion

                            #region 去掉服务器内存中客户端信息
                            for (int i = socketList.Count - 1; i >= 0; i--)
                            {
                                if (tempList[0].SocketForm == socketList[i].SocketForm)
                                {
                                    socketList.Remove(socketList[i]);
                                    Console.WriteLine("关闭链接");
                                }
                            }
                            #endregion

                            #region 记录客户端下线的日志（TO-DO）

                            #endregion
                        }
                    };

                    //图片处理
                    socket.OnBinary = dataarray => {

                        //获取发送的socket
                        SocketClient messageFormClient = (from r in socketList where r.TheSocketClient == socket select r).ToList<SocketClient>()[0];

                        string messageTo = messageFormClient.SocketTo; //例如：发往03

                        SocketClient messageToClient = (from r in socketList where r.SocketForm == messageTo select r).ToList<SocketClient>()[0];


                        messageToClient.TheSocketClient.Send(dataarray);



                    };

                    //消息处理(文本消息)
                    socket.OnMessage = message =>
                    {
                        //解析JSON
                        MessageDataModel messageData = JsonConvert.DeserializeObject<MessageDataModel>(message);
                        if (messageData.message_type == "init")
                        {
                            int exist_count = (from r in socketList where r.TheSocketClient == socket select r).Count();
                            if (exist_count == 0)
                            {
                                //后台数据记录登录的状态（socket_id,patient_id,登录时间）
                                SocketClient client = new SocketClient();

                                client.TheSocketClient = socket;
                                client.SocketForm = messageData.message_from;
                                client.SocketTo = messageData.message_to;

                                socketList.Add(client);

                                Console.WriteLine("登录信息：" + messageData.message_from);

                                //推送登录信息
                                if (!string.IsNullOrEmpty(client.SocketTo))
                                {
                                    var purposeClinetList = (from r in socketList where r.SocketForm == client.SocketTo select r);
                                    if (purposeClinetList.Count() == 1)
                                    {
                                        SocketClient purposeClient = purposeClinetList.ToList<SocketClient>()[0];
                                        purposeClient.TheSocketClient.Send(message);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("重复登录操作：" + messageData.message_from);

                                for (int i = 0; i < socketList.Count; i++)
                                {
                                    if (socket == socketList[i].TheSocketClient)
                                    {
                                        socketList[i].SocketForm = messageData.message_from;
                                        socketList[i].SocketTo = messageData.message_to;
                                    }
                                }
                            }
                        }

                        if (messageData.message_type == "normal")
                        {
                            Console.WriteLine("发送的消息：" + messageData.message_content + ",消息来自：" + messageData.message_from + "，消息要给：" + messageData.message_to);

                            SocketClient messageFormClient = (from r in socketList where r.TheSocketClient == socket select r).ToList<SocketClient>()[0];

                            string messageFom = messageFormClient.SocketForm; //例如：来自01
                            string messageTo = messageFormClient.SocketTo; //例如：发往03


                            var messageToList = (from r in socketList where r.SocketForm == messageTo select r).ToList<SocketClient>();

                            messageToList.Add(messageFormClient);

                            messageToList.ForEach(t => {
                                t.TheSocketClient.Send(message);
                            });
                        }
                    };
                });

                var input = Console.ReadLine();
                while (input != "exit")
                {

                    input = Console.ReadLine();
                }
            }

        }

    public class SocketClient
    {
        public IWebSocketConnection TheSocketClient { get; set; }
        public string SocketForm { get; set; }
        public string SocketTo { get; set; }

    }

    public class MessageDataModel
    {
        public string message_type { get; set; }
        public string message_from { get; set; }
        public string message_to { get; set; }
        public string message_content { get; set; }
    }

}



