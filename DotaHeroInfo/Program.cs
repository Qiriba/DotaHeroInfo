using OpenCvSharp;
using System;
using System.Drawing;
using System.IO;

class Program
{
    static void Main()
    {
        Thread.Sleep(5000);
        // Define the region where hero portraits are displayed (adjust for your screen)
        Rectangle heroRegion = new Rectangle(208, 0, 1500, 74);  // Adjust this to your hero portrait area

        // Capture the screen region containing the hero portraits
        Bitmap croppedImage = CaptureWindowRegion(heroRegion);

        // Convert the cropped image to OpenCV Mat format
        Mat mat = BitmapToMat(croppedImage);

        // Define the path to your hero portraits folder
        string heroImagesPath = @"C:\Users\Tobias\Pictures\Hero displays";  // Set the correct path to your hero images folder

        // Loop through each hero image in the folder and perform template matching
        string[] heroFiles = Directory.GetFiles(heroImagesPath, "*.png");  // Change to .jpg or .bmp if needed

        foreach (var heroFile in heroFiles)
        {
            using (Mat heroImage = Cv2.ImRead(heroFile, ImreadModes.Color))
            {
                // Resize the hero image to match the icon size (68x41)
                Mat resizedHeroImage = new Mat();
                Cv2.Resize(heroImage, resizedHeroImage, new OpenCvSharp.Size(123, 70));

                // Perform template matching to compare the current hero image with the screenshot
                Mat result = new Mat();
                Cv2.MatchTemplate(mat, resizedHeroImage, result, TemplateMatchModes.CCoeffNormed);

                // Find the best match position
                double minVal, maxVal;
                OpenCvSharp.Point minLoc, maxLoc;
                Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);

                if (maxVal > 0.6)  // Threshold for a "good" match, adjust as needed
                {
                    Console.WriteLine($"Matched Hero: {Path.GetFileNameWithoutExtension(heroFile)} with score: {maxVal}");
                }

                // Dispose of resized hero image to free resources
                resizedHeroImage.Dispose();
            }
        }
        SaveScreenshotToFile(croppedImage, @"D:\test.png");
        // Dispose of the cropped image
        croppedImage.Dispose();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    // Capture only the required region from the screen
    public static Bitmap CaptureWindowRegion(Rectangle region)
    {
        Bitmap screenshot = new Bitmap(region.Width, region.Height);
        using (Graphics g = Graphics.FromImage(screenshot))
        {
            g.CopyFromScreen(region.Left, region.Top, 0, 0, new System.Drawing.Size(region.Width, region.Height), CopyPixelOperation.SourceCopy);
        }
        return screenshot;
    }

    // Convert a Bitmap to Mat (OpenCV format)
    public static Mat BitmapToMat(Bitmap bitmap)
    {
        using (var ms = new System.IO.MemoryStream())
        {
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
        }
    }
public static void SaveScreenshotToFile(Bitmap screenshot, string filePath)
{
    screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
    Console.WriteLine($"Screenshot saved to {filePath}");
}
}
