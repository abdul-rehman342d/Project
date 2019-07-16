using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PushSharp.Windows;

namespace TestProject.Notification
{
    public class ToastAudio
    {
        public ToastAudioSource Source { get; set; }

        public bool Loop { get; set; }

        public virtual XElement GenerateXmlElement()
        {
            XElement xelement = new XElement((XName)"audio");
            if (this.Source == ToastAudioSource.Silent)
            {
                xelement.Add((object)new XAttribute((XName)"silent", (object)"true"));
            }
            else
            {
                if (this.Loop)
                    xelement.Add((object)new XAttribute((XName)"loop", (object)"true"));
                if (this.Source != ToastAudioSource.LoopingCall)
                {
                    string str = (string)null;
                    switch (this.Source)
                    {
                        case ToastAudioSource.Mail:
                            str = "ms-winsoundevent:Notification.Mail";
                            break;
                        case ToastAudioSource.SMS:
                            str = "ms-winsoundevent:Notification.SMS";
                            break;
                        case ToastAudioSource.IM:
                            str = "ms-winsoundevent:Notification.IM";
                            break;
                        case ToastAudioSource.Reminder:
                            str = "ms-winsoundevent:Notification.Reminder";
                            break;
                        case ToastAudioSource.LoopingCall:
                            str = "ms-winsoundevent:Notification.Looping.Call";
                            break;
                        case ToastAudioSource.LoopingCall2:
                            str = "ms-winsoundevent:Notification.Looping.Call2";
                            break;
                        case ToastAudioSource.LoopingAlarm:
                            str = "ms-winsoundevent:Notification.Looping.Alarm";
                            break;
                        case ToastAudioSource.LoopingAlarm2:
                            str = "ms-winsoundevent:Notification.Looping.Alarm2";
                            break;
                    }
                    xelement.Add((object)new XAttribute((XName)"src", (object)str));
                }
            }
            return xelement;
        }
    }
}
