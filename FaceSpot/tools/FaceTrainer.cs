
using System;
using FSpot.Utils;

using NeuronDotNet.Core;
using NeuronDotNet.Core.Backpropagation;
using FaceSpot.Db;

namespace FaceSpot	
{	

	public class FaceTrainer
	{				
		
		public static void Train(Face[] faces){
			TrainNetwork(EigenRecogizer.ProcessPCA(faces));
		}
		
		/// <summary>
		/// train and save as the spcified path
		/// </summary>
		/// <param name="eigen">
		/// A <see cref="EigenValueTags"/>
		/// </param>
		private static void TrainNetwork(EigenValueTags eigen){
			Log.Debug("Train Started...");
			
			string[] dLabels = eigen.FacesLabel;//FindDistinctClass(eigen);						
			int numInstances = eigen.eigenTaglist.Count;
			int inputNodes = eigen.eigenTaglist[0].val.Length;					
			int outputNodes = dLabels.Length;
			int hiddenNodes = inputNodes+outputNodes;
			
			float[][] trainInput = new float[numInstances][];
			float[][] trainOutput = new float[numInstances][];
			
			//Random r = new Random();
			int numstrain = 0;
			for(int i=0;i<numInstances;i++){				
				
				trainInput[numstrain] = new float[inputNodes];
				trainOutput[numstrain] = new float[outputNodes];
				
				for(int j=0;j<dLabels.Length;j++){
					if(eigen.eigenTaglist[i].tag.Equals(dLabels[j]))
						trainOutput[numstrain][j] = 0.9f;
					else
						trainOutput[numstrain][j] = 0.1f;
				}
				
				for(int j=0;j<inputNodes;j++){					
					trainInput[numstrain][j] = eigen.eigenTaglist[i].val[j];
				}
				numstrain++;
			}
			
			// convert to double
			Log.Debug("nums train = "+ numstrain);
			double[][] trainInputD = new double[numstrain][];
			double[][] trainOutputD = new double[numstrain][];
			for(int i=0;i<numstrain;i++){				
				trainInputD[i] = new double[inputNodes];
				trainOutputD[i] = new double[outputNodes];
				for(int j=0;j<outputNodes;j++){
					trainOutputD[i][j] = trainOutput[i][j];
				}
				
				for(int j=0;j<inputNodes;j++){	
					trainInputD[i][j] = trainInput[i][j];
				}
			}						 					     													
					
			TimeSpan tp = System.DateTime.Now.TimeOfDay;	
			
			NeuronDotNet.Core.Backpropagation.SigmoidLayer inputLayer = new NeuronDotNet.Core.Backpropagation.SigmoidLayer(inputNodes);
			NeuronDotNet.Core.Backpropagation.SigmoidLayer hiddenlayer = new NeuronDotNet.Core.Backpropagation.SigmoidLayer(hiddenNodes);
			NeuronDotNet.Core.Backpropagation.SigmoidLayer outputlayer = new NeuronDotNet.Core.Backpropagation.SigmoidLayer(outputNodes);
			
			BackpropagationConnector input_hidden =  new BackpropagationConnector(inputLayer, hiddenlayer);
			BackpropagationConnector hidden_output =  new BackpropagationConnector(hiddenlayer, outputlayer);
			
			input_hidden.Momentum = 0.3;
			hidden_output.Momentum = 0.3;
			
			BackpropagationNetwork bpnet = new BackpropagationNetwork(inputLayer,outputlayer);
			TrainingSet tset = new TrainingSet(inputNodes, outputNodes);			
			for(int i=0;i<numstrain;i++)
				tset.Add(new TrainingSample(trainInputD[i], trainOutputD[i]));
			
			// prevent getting stuck in local minima
			bpnet.JitterNoiseLimit = 0.0001;
			bpnet.Initialize();
			
			int numEpoch = 100;			
			bpnet.SetLearningRate(0.2);
			bpnet.Learn(tset, numEpoch);
						
//			string savepath = facedbPath + "object/";
//			if(!Directory.Exists(savepath))
//				Directory.CreateDirectory(savepath);
			
			// Serialize
			SerializeUtil.Serialize("/home/hyperjump/nn.dat", bpnet);
			// Deserialize
			//bpnet = (BackpropagationNetwork)SerializeUtil.DeSerialize("nn.dat");
			
			// test by using training data
			int correct = 0;			
			for(int i=0;i<numInstances;i++){
				
				double[] v = new double[inputNodes];
				for(int j=0;j<v.Length;j++)
					v[j] = (double)eigen.eigenTaglist[i].val[j];
				
				string result = FaceClassifier.AnalyseNetworkOutput(eigen, bpnet.Run(v));
				if(result.Equals(eigen.eigenTaglist[i].tag))
					correct++;				
			}
			Log.Debug("% correct = " + (float)correct/(float)numInstances * 100);
			Log.Debug("time ="+  System.DateTime.Now.TimeOfDay.Subtract(tp));											
			
			Log.Debug("Train ended...\n");
		}				
	}
}
