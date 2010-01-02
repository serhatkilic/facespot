
using System;
using System.Collections.Generic;
using FaceSpot.Db;
using FSpot;
using FSpot.Utils;
using Banshee.Kernel;
namespace FaceSpot
{
	//TODO Add FaceSpot Preference
	public class FaceScheduler
	{
		private static FaceScheduler instance;
		public static FaceScheduler Instance{
			get {
				if(instance == null)
				{
					instance = new FaceScheduler();
				}
				return instance;
			}
		}
		
		private FaceScheduler ()
		{
			Scheduler.JobFinished += SchedulerJobFinished;	
			//Scheduler.JobStarted += SchedulerJobStarted;
		}


		void SchedulerJobFinished (IJob job)
		{
			if( job is DetectionJob){
				DetectionJob dJob = (DetectionJob) job;
				Log.Debug("DJob Finished Event "+dJob.JobOptions);
				foreach(Face face in dJob.DetectedFaces){
					RecognitionJob.Create(face,dJob.priority);
				}
			}
			if( job is RecognitionJob){
				
				
			}
			
			Execute();
		}
		
		public void Execute(){
			if(Scheduler.ScheduledJobsCount == 0)
			{
				//MainWindow.Toplevel.PhotoView.
				QueueAnyUncheckedPhoto();	
			}
		}
		const int QUEUE_ENTRY_LIMIT = 5;
		public void QueueAnyUncheckedPhoto()
		{
			Photo[] undetectedPhotos = FaceSpotDb.Instance.PhotosAddOn.GetUnDetectedPhoto();
			int i=0;
			foreach( Photo photo in undetectedPhotos)	
			{
				//DetectionJob job = 
				DetectionJob.Create(photo);
				if(i++ == QUEUE_ENTRY_LIMIT) break;
			}
			i=0;
			Face[] unRecognizedFace = FaceSpotDb.Instance.Faces.GetNotRecognizedFace();
			Log.Debug("Unrecognized Face : "+unRecognizedFace.Length);
			foreach( Face face in unRecognizedFace){
				RecognitionJob.Create(face);
				if(i++ == QUEUE_ENTRY_LIMIT) break;
			}
		}
	}
}
