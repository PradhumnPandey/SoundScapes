using System.Drawing;
using SoundScapesEncoder;
namespace SoundScapes
{
    public class Main
    {
		private int height;
		private int width;
        public Bitmap readImage(string path)
        {
			try
			{
				if(File.Exists(path))
				{
					Bitmap bitmap = new Bitmap(path);
					height = bitmap.Height;
					width = bitmap.Width;
					return bitmap;
				}
				else
				{
					throw new FileNotFoundException();
				}
			}
			catch (Exception)
			{

				throw;
			}
        }
		public void generateAudio(string path)
		{
			try
			{
				using(StreamReader sr = new StreamReader(path))
				{

				}
			}
			catch (Exception)
			{

				throw;
			}
		}
		public void generateImage(string path)
		{
			int height, width;
            string line = String.Empty;
			Bitmap bitmap = null;
			string[] pixel;
			using(StreamReader sr = new StreamReader(path))
			{
				line = sr.ReadLine();
				String[] dim = line.Split(" "); ;
				String[] pixelData;
				int A,R,G,B;
				height = Convert.ToInt32(dim[0]);
				width = Convert.ToInt32(dim[1]);
				bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                for (int i = 0; i < width; i++)
                {
					if((line = sr.ReadLine()) != null)
					{
                        pixel = line.Split(",");
                        for (int j = 0; j < height; j++)
                        {
                            pixelData = pixel[j].Split(" ");
                            A = Convert.ToInt32(pixelData[0]);
                            R = Convert.ToInt32(pixelData[1]);
                            G = Convert.ToInt32(pixelData[2]);
                            B = Convert.ToInt32(pixelData[3]);

                            Color pix = Color.FromArgb(A, R, G, B);

                            bitmap.SetPixel(i, j, pix);
                        }
                    }
                }

            }
			bitmap.Save(Directory.GetCurrentDirectory() + "\\img.jpeg",System.Drawing.Imaging.ImageFormat.Jpeg);
			
		}

        public void generateImageBW(string path)
        {
            string line = String.Empty;
            Bitmap bitmap = null;
            string[] stream;
            int row = 0, col = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                row = 0;
                while((line = sr.ReadLine()) != null)
                {
                    col = 0;
                    if (row >= height || col >= width) break;
                    line = line.Trim();
                    stream = line.Split(" ");
                    foreach(String i in stream)
                    {
                        Color pix;
                        if (i == "0")
                            pix = Color.FromArgb(0, 0, 0);
                        else
                            pix = Color.FromArgb(255, 255, 255);

                        bitmap.SetPixel(col, row, pix);
                        col++;
                    }
                    row++;
                }
               
            }
            bitmap.Save(Directory.GetCurrentDirectory() + "\\imgBW.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);

        }

        public Bitmap readImageBW(string path)
		{
            try
            {
                if (File.Exists(path))
                {
                    Bitmap bitmap = new Bitmap(path);
                    height = bitmap.Height;
                    width = bitmap.Width;
                    return bitmap;
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

		public void generatePixelDataBW(Bitmap bitmap)
		{
            try
            {
                double hsp;// hsp equation use krrha hu... dark or ligt ke liye
                using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\Output\\pixelDataBW.txt"))
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            Color pixel = bitmap.GetPixel(j, i);
                            hsp = Math.Sqrt(0.299 * (pixel.R * pixel.R) + 0.587 * (pixel.G * pixel.G) + 0.114 * (pixel.B * pixel.B));
                            if (hsp > 127.5)
                                sw.Write("1 ");
                            else
                                sw.Write("0 ");
                        }
                        sw.WriteLine();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public void generatePixelData(Bitmap bitmap)
		{
			try
			{
				using(StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\Output\\pixelData.txt")) 
				{
					sw.WriteLine(width + " " + height);
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Color pixel = bitmap.GetPixel(i, j);
							sw.Write(pixel.A + " " + pixel.R + " " + pixel.G + " " + pixel.B + ",");
                        }
						sw.WriteLine();
                    }
                }
            }
            catch (Exception)
			{

				throw;
			}

		}
        public Main(string path, int option, int type)
        {
            if(type == 1)
            {
                BWEncoder encoder = new BWEncoder();
                if (option == 1)
                {
                    Bitmap bitmap = readImageBW(path);
                    generatePixelDataBW(bitmap);
                    encoder.EncodeBW(Directory.GetCurrentDirectory() + "\\Output\\pixelDataBW.txt");
                }
                else
                {
                    int[]dimension = encoder.readAudioBW(Directory.GetCurrentDirectory() + "\\output.wav");
                    this.width = dimension[0];
                    this.height = dimension[1];
                    generateImageBW(Directory.GetCurrentDirectory() + "\\Output\\convertedData.txt");
                }
            }
        }
    }
}