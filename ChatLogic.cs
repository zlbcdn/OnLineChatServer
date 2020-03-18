using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnChatServer
{
    public class ChatLogic
    {

        //获取

    }


    public class MessageContentModel
    {
        public int MessageID { get; set; }
        public string MessageFromID { get; set; }
        public string MessageFromName { get; set; }
        public DateTime MessageDate { get; set; }
        public string MessageToID { get; set; }
        public string MessageToName { get; set; }
        public int MessageType { get; set; }
        public string MessageContent { get; set; }
        public int MessageStatus { get; set; }
    }

}
