#region Using statements
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using RightEdge.Common;
using RightEdge.Indicators;
#endregion

/**
* Data for a certain strategy and bar, e.g. donchian, 5th bar
* Holds all relativeMFE and relativeMAE. Their (divided) average will be 
* the e-ratio for a certain day and strategy.
*/
public class BarStats {
	
	public List<double> relativeMFE = new List<double>();
	public List<double> relativeMAE = new List<double>();
	
	public int getDataCount() {
		return relativeMFE.Count;
	}
	
	public double getERatio() {
		
		if( relativeMFE.Count != relativeMAE.Count ) {
			Console.WriteLine( "MFE != MAE length" );
			return 0;
		}
		
		double mfeSum = 0, maeSum = 0;
		int counter = 0;
		
		for( int i = 0; i < relativeMFE.Count; i++ ) {
			
			counter++;
			mfeSum += relativeMFE[ i ];
			maeSum += relativeMAE[ i ];
			
		}
		
		return mfeSum / maeSum;
		
	}
	
}