
using System;
using FSpot;
using Gdk;
using FSpot.Widgets;
using FSpot.Utils;
	
namespace FaceSpot.Db
{
	public class Face : DbItem, IDisposable
		//TODO Decide whether should it implement IComparable
	{
		uint faceID;		
		
		uint leftX,topY,width;	
		#region Position/Width Encapsulation
		public uint LeftX {
			get {
				return leftX;
			}
			set {
				leftX = value;
			}
		}

		public uint TopY {
			get {
				return topY;
			}
			set {
				topY = value;
			}
		}

		public uint Width {
			get {
				return width;
			}
			set {
				width = value;
			}
		}
		#endregion
		//TODO design how to add thumbnails
		const int faceDefaultWidth = 100;
		public Pixbuf faceImage;
		
		bool manuallyDetected, manuallyRecognized;
		
		string photo_md5;
		
		public Photo photo;
		
		public Pixbuf iconPixbuf;
		public bool iconWasCleared;
		/// <summary>
		/// 
		/// </summary>
		public Tag tag;
		/// <summary>
		/// 
		/// </summary>
		public bool tagConfirmed;
		
		public Face (uint id,uint leftX,uint topY,uint width,Photo photo,Pixbuf icon) 
			: base (id)
		{
			this.faceID = id;
			this.leftX = leftX;
			this.topY = topY;
			this.width = width;
			this.photo = photo;
			this.iconPixbuf = icon;
			//FIXME Possible Error HERE
			photo_md5 = photo.MD5Sum;
			
			
		}
		
		//TODO Add Function for move/scale(1:1) Face
		
		public void Dispose()
		{
			//TODO Add required child item dispose
			
			System.GC.SuppressFinalize(this);
		}
		public static PixbufCache getFaceImageFromPhoto(uint left,uint top,uint width, Photo photo)
		{
			throw new NotImplementedException();	
		}
	}
}
