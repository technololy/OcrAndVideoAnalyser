using System;
using System.Collections;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Repository
{
    public interface ICamuDataTableRepository
    {
        List<Models.Camudatafield> GetCamuData();
        Models.Camudatafield GetCamuDataByID(int CamuDataId);
        void InsertCamuData(Models.Camudatafield CamuData);
        void DeleteCamuData(int CamuDataId);
        void UpdateCamuData(Models.Camudatafield CamuData);
        void Save();
    }
}
