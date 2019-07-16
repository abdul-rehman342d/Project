using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PushSharp.Windows;

namespace TestProject.Notification
{
    public class WindowsToastNotification : WindowsNotification
    {
        public override WindowsNotificationType Type
        {
            get
            {
                return WindowsNotificationType.Toast;
            }
        }

        public string Launch { get; set; }

        public ToastDuration Duration { get; set; }

        public PushSharp.Windows.ToastAudio Audio { get; set; }

        public ToastVisual Visual { get; set; }

        public WindowsToastNotification()
        {
            this.Visual = new ToastVisual();
        }

        public override string PayloadToString()
        {
            XElement xelement = new XElement((XName)"toast");
            if (!string.IsNullOrEmpty(this.Launch))
                xelement.Add((object)new XAttribute((XName)"launch", (object)this.XmlEncode(this.Launch)));
            if (this.Duration != ToastDuration.Short)
                xelement.Add((object)new XAttribute((XName)"duration", (object)this.Duration.ToString().ToLowerInvariant()));
            if (this.Audio != null)
                xelement.Add((object)this.Audio.GenerateXmlElement());
            if (this.Visual != null)
                xelement.Add((object)this.Visual.GenerateXmlElement());
            return ((object)xelement).ToString();
        }
    }
}
