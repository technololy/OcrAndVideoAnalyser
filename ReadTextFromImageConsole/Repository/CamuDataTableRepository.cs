using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ReadTextFromImageConsole.Models;

namespace ReadTextFromImageConsole.Repository
{
    public class CamuDataTableRepository : ICamuDataTableRepository
    {
        private AppDbContext context;

        public CamuDataTableRepository()
        {
            context = new AppDbContext();


        }

        public void DeleteCamuData(int CamuDataId)
        {
            throw new NotImplementedException();
        }

        public List<Models.Camudatafield> GetCamuData()
        {
            return context.Camudatafield.ToList();

        }

        public Camudatafield GetCamuDataByID(int CamuDataId)
        {
            throw new NotImplementedException();
        }

        public void InsertCamuData(Camudatafield CamuData)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void UpdateCamuData(Camudatafield CamuData)
        {
            context.Entry(CamuData).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

        }
    }
}
