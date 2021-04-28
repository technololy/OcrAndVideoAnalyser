using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcctOpeningImageValidationAPI.Repository.Abstraction
{
    public interface IFaceRepository
    {
        void ExtractFrameFromVideo(string directory, string fiileName);
        Task<Tuple<bool, bool, bool, bool>> RunHeadGestureOnImageFrame(string filePath);
    }
}
