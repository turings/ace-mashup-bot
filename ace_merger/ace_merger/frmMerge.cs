using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AceMerger
{
    public partial class frmMerge : Form
    {

        public frmMerge()
        {
            InitializeComponent();
            // Grab heads and bodies
            // Initialise directories
            DirectoryInfo headDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Images\Heads");
            DirectoryInfo bodyDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Images\Bodies");
            // Grab a list of file paths
            string[] heads = headDirectory.GetFiles().Select(x => x.FullName).ToArray();
            string[] bodies = bodyDirectory.GetFiles().Select(x => x.FullName).ToArray();
            // Make all possible combinations
            for(int h = 0; h < heads.Length; h++)
            {
                for (int b = 0; b < bodies.Length; b++)
                {
                    string headName = Path.GetFileName(heads[h]);
                    string bodyName = Path.GetFileName(bodies[b]);
                    // Never merge a head with the right body!
                    if (headName != bodyName)
                        Merge(heads[h], bodies[b]);

                }
            }
        }

        private void Merge(string headPath,
            string bodyPath)
        {
            // Load courtroom background, with box bar to overlay over character
            Bitmap courtroom = LoadImage(AppDomain.CurrentDomain.BaseDirectory + @"\Images\Backgrounds\Courtroom.png");
            Bitmap box = LoadImage(AppDomain.CurrentDomain.BaseDirectory + @"\Images\Backgrounds\Box.png");
            // Load head and body images
            Bitmap head = LoadImage(headPath);
            Bitmap body = LoadImage(bodyPath);
            string headName = Path.GetFileNameWithoutExtension(headPath);
            string bodyName = Path.GetFileNameWithoutExtension(bodyPath);
            // 18/05: Won't split this til it's been passed through the bot, as need full names for alt-text
            //// Get the first name of the head, and the last name of the body (provided there are two names)
            //if(headName.Contains("!"))
            //{
            //    headName = headName.Substring(0, headName.IndexOf("!"));

            //}
            //if (bodyName.Contains("!"))
            //{
            //    int index = bodyName.IndexOf("!");
            //    bodyName = bodyName.Substring(index + 1, bodyName.Length - index - 1);
            //}
            // Merge images
            var target = new Bitmap(courtroom.Width, courtroom.Height);
            var graphics = Graphics.FromImage(target);
            graphics.CompositingMode = CompositingMode.SourceOver;
            // Make the courtroom the background image
            graphics.DrawImage(courtroom, 0, 0, courtroom.Width, courtroom.Height);
            // Position the body in the middle of the canvas at the bottom, above witness box line (20 px up)
            graphics.DrawImage(body, (courtroom.Width / 2) - (body.Width / 2), (courtroom.Height - body.Height) - 20, body.Width, body.Height);
            // Position head in the middle of the canvas, over the body
            graphics.DrawImage(head, (courtroom.Width / 2) - (head.Width / 2), (courtroom.Height - body.Height - head.Height) + 1, head.Width, head.Height);
            // Position box bar over character to make it look as though they're in the witness box
            graphics.DrawImage(box, 0, 0, courtroom.Width, courtroom.Height);
            target.Save(@"Monstrosities\" + headName + "&" + bodyName + ".png", ImageFormat.Png);
            target.Dispose();
            graphics.Dispose();
            courtroom.Dispose();
            box.Dispose();
            head.Dispose();
            body.Dispose();
        }

        private Bitmap LoadImage(string path)
        {
            try
            {
                return new Bitmap(path);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
