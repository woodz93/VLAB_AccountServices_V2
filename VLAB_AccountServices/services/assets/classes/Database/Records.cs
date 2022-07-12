using System.Collections.Generic;
namespace VLAB_AccountServices.services.assets.classes.Database {
	public class Records {
		public List<Dictionary<string,string>> Raw=null;
		private int Iter=0;
		public Records(List<Dictionary<string,string>> obj=null) {
			this.Raw=obj;
		}
		// Resets the iteration counter variable.
		public void Reset() {
			this.Iter=0;
		}
		// Clears the results.
		public void Clear() {
			this.Raw.Clear();
			this.Iter=0;
		}
		// Prepares an iteration process.
		public bool Read() {
			//Dictionary<string,string> res=new Dictionary<string, string>();
			bool res=true;
			if (this.Raw==null) {
				res=false;
			} else {
				if (!(this.Iter<this.Raw.Count)) {
					res=false;
				} else {
					//res=this.Raw[this.Iter];
					this.Iter++;
				}
			}
			return res;
		}
		// Returns the length/number of records that were found.
		public int Count() {
			int res=0;
			if (this.Raw!=null) {
				res=this.Raw.Count;
			}
			return res;
		}
		// Returns a dictionary value consisting of the row's columns and values.
		public Dictionary<string,string> GetRow(int row=-1) {
			Dictionary<string,string> res=null;
			if (row>=0) {
				if (row<this.Raw.Count) {
					res=this.Raw[row];
				}
			}
			return res;
		}
		// Returns a string representation of the object.
		public string ToString() {
			return "[Database.RecordsObject]";
		}
	}
}