using System;

namespace MLG2007.Helper.ListSearch
{
	/// <summary>
	/// Summary description for ListPlannerData.
	/// </summary>
	public class ListPlannerData: ListData
	{
		public ListPlannerData()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected override string BuildQuery()
		{
			string query="";
			query = "<Where>";
			query += "<Or>";
			query += "<And>";
			query += "<Leq><FieldRef Name=\"EventDate\"/><Value Type=\"DateTime\">"+Values[0].ToString()+"Z</Value></Leq>";
			query += "<Geq><FieldRef Name=\"EventDate\"/><Value Type=\"DateTime\">"+Values[1].ToString()+"Z</Value></Geq>";
			query += "</And>";
			query += "<And>";
			query += "<Leq><FieldRef Name=\"EventDate\"/><Value Type=\"DateTime\">"+Values[0].ToString()+"Z</Value></Leq>";
			query += "<Geq><FieldRef Name=\"EndDate\"/><Value Type=\"DateTime\">"+Values[1].ToString()+"Z</Value></Geq>";
			query += "</And>";
			query += "</Or>";
			query += "</Where>";
			return query;
			//return base.BuildQuery ();
		}
	}
}
