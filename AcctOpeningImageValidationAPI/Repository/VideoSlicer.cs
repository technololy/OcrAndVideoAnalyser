using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System;
using System.Drawing;
using System.IO;

namespace AcctOpeningImageValidationAPI.Repository
{
    public class VideoSlicer
    {
        public Tuple<bool, string> ExtractFrameFromVideo(string directory, string fiileName)
        {
            //Initiatlize Medial Took Kit to Extracting image(frame) from video
            var mp4 = new MediaFile { Filename = Path.Combine(directory, fiileName) };
            using var engine = new Engine();

            //Getting Meta Data
            engine.GetMetadata(mp4);

            //Initializing Seek to One
            var i = 0;

            if (mp4.Metadata.Duration.TotalSeconds < 1)
            {
                return new Tuple<bool, string>(false, "Face capturing must be 15 seconds long, please try again");
            }
            //Looping through
            //TODO: This should be limited to 9 Seconds video
            while (i < mp4.Metadata.Duration.Seconds)
            {
                //Conversion Options Settings
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i), };

                //Constructing the output file
                var outputFile = new MediaFile { Filename = string.Format("{0}\\image-{1}.jpeg", Path.Combine(directory), i) };

                //This Method is native to Window OS, this line will fail on (i.e. Macintosh & Docker Container)
                //Actual extraction of image
                engine.GetThumbnail(mp4, outputFile, options);

                //This is weird, but we've got to rotate it
                //Discalimer: DO NOT COMMENT, THIS IS THE PILLAR OF THE ENTIRE PROCESS!!!
                RotateImage(outputFile.Filename);
                i++;
            }

            return new Tuple<bool, string>(true, "Video analysis was successful");
        }

        /// <summary>
        /// This method flip image orientation by 90 degree
        /// Please DO NOT ALTER this method!!!
        /// </summary>
        /// <param name="path"></param>
        private void RotateImage(string path)
        {
            Image image = Image.FromFile(path);
            image.RotateFlip(RotateFlipType.Rotate90FlipY);
            File.Delete(path);
            image.Save(path);
        }
    }
}
