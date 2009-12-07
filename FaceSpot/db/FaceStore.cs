
using System;
using Banshee.Database;
using Mono.Data.SqliteClient;
using FSpot.Utils;
using FSpot;
using FSpot.Widgets;
using FSpot.Extensions;

namespace FaceSpot.Db
{
	public class FaceStore : DbStore<Face>
	{
		const string ALL_FIELD_NAME = "id, photo_id, tag_id, tag_confirm, left_x, top_y, width, photo_md5 ";
		public FaceStore (QueuedSqliteDatabase database, bool is_new)
			: base(database, false)
		{
			//TODO Add Ensure FaceThumbnailDirectory ?? (Similar to PhotoStore.cs)(
			
			//FIXME Detect Whether faces table has been initialized on the database
			if ( ! is_new) return;
			//Add Database Initialization
			//Note : If you change query here - you need to change "all_field" too
			Database.ExecuteNonQuery(
				"CREATE TABLE faces (\n"+
			    "	id INTEGER PRIMARY KEY NOT NULL, \n" +
			    "	photo_id INTEGER NOT NULL, \n"+
				" 	tag_id INTEGER NOT NULL, \n" +
				"   tag_confirm BOOLEAN, \n"+
			    "	left_x INTEGER NOT NULL, \n"+
			    "	top_y INTEGER NOT NULL, \n"+
			    "	width INTEGER NOT NULL, \n"+
			    " 	photo_md5 STRING NOT NULL, \n"
			                         );
			
			//TODO Add Database Index / Links
			Database.ExecuteNonQuery("CREATE INDEX idx_photo_id ON faces(photo_id)");
			Database.ExecuteNonQuery("CREATE INDEX idx_photo_id ON faces(tag_id)");
		}
		
		public override Face Get (uint id)
		{
			Face face = LookupInCache (id);
			if (face != null)
				return face;
			SqliteDataReader reader = Database.Query (
			new DbCommand ("SELECT " + ALL_FIELD_NAME + 
				      "FROM faces " + 
				      "WHERE id = :id", "id", id
				     )
			);
			if (reader.Read ())
			{
				Photo photo = Core.Database.Photos.Get((uint)Convert.ToUInt32(reader["photo_id"]));
				face = new Face(id,
				                Convert.ToUInt32(reader["left_x"]),
				                Convert.ToUInt32(reader["top_Y"]),
				                Convert.ToUInt32(reader["width"]),
				               	photo
				                ) ;
				AddToCache(face);
			}
			reader.Close();
			//TODO consider whether to use Unsafed Add (Compared to PhotoStore Class)
			return face;
		}
		
		public Face CreateFace (Photo photo, uint leftX, uint topY, uint width)
		{
			throw new System.NotImplementedException ();
			//return null;
		}
		
 		public override void Commit (Face item)
 		{
			Commit(new Face[]{item});
		}
		
		public void Commit (Face[] items){
			uint timer = Log.DebugTimerStart ();
			// TODO consider whether we need to change any more information
			// Only use a transaction for multiple saves. Avoids recursive transactions.
 			bool use_transactions = !Database.InTransaction && items.Length > 1;
			if(use_transactions) Database.BeginTransaction();
//			foreach (Face face in items){
//				Database.ExecuteNonQuery(
//					new DbCommand("UPDATE faces SET photo_id = :photo_id"+
//					", tag_id = :tag_id, tag_confirm = :tag_confirm, left_x = :left_x, top_y = :top_y,"+
//					"width = :width , photo_md5 = :photo_md5 WHERE id= :id",
//						"photo_id", face.photo.Id,
//						"tag_id",face.tag.Id,
//						"tag_confirm", face.tagConfirmed,
//						"left_x",face.LeftX,
//						"top_y",face.TopY,
//						"width",face.Width,
//						"photo_md5",face.ph
//						
//				
//			}
			if(use_transactions) Database.CommitTransaction();
			Log.DebugTimerPrint (timer, "Commit took {0}");
 		}
		
		public uint AddTag (Tag tag)
		{
			throw new System.NotImplementedException ();
		}
		
		public override void Remove (Face item)
		{
			//TODO Add this
			throw new System.NotImplementedException ();
		}
		
		//TODO Add more Query
		
		//TODO Add "Emit" classes
	}
}
