using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoursePlanner_RevF
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
