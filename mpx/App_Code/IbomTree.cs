using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


/// <summary>
/// Summary description for IbomTree
/// </summary>
public class IbomTree
{
	public string name;
    public string imgsource;
    
    public IbomTree() : this("picture.jpg") {
		
	}

    public IbomTree(string name) {
        this.name = name;
    }

    public IbomTree(string name, string imgsource): this(name) {
        this.imgsource = imgsource;
    }

    public void Draw() {
        int height = 100;
        int width = 200;
        Random r = new Random();
        int x = r.Next(75);

        Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        Graphics g = Graphics.FromImage(bmp);

        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        g.Clear(Color.Orange);
        g.DrawRectangle(Pens.White, 1, 1, width - 3, height - 3);
        g.DrawRectangle(Pens.Gray, 2, 2, width - 3, height - 3);
        g.DrawRectangle(Pens.Black, 0, 0, width, height);
        g.DrawString("The Code Project", new Font("Arial", 12, FontStyle.Italic),
        SystemBrushes.WindowText, new PointF(x, 50));

        bmp.Save(name, ImageFormat.Jpeg);

        g.Dispose();
        bmp.Dispose();
    }
}