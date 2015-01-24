using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using RightEdge.Common;
using RightEdge.Indicators;


/**
* Statistics for a certain strategy, e.g. donchian20.
* - bar1, BarStats
* - bar2, BarStats
*/
public class StrategyStats {
	
	// days -> stats
	public Dictionary<int, BarStats> barStats = new Dictionary<int, BarStats>();
		
	public void addData( int barCount, double mfe, double mae ) {
				
		BarStats stat;
		if( barStats.TryGetValue( barCount, out stat ) ) {
			stat.relativeMFE.Add( mfe );
			stat.relativeMAE.Add( mae );
		}
		else {
			BarStats newStat = new BarStats();
			newStat.relativeMFE.Add( mfe );
			newStat.relativeMAE.Add( mae );
			barStats.Add( barCount, newStat );
		}
		
	}
	
	public void logData() {
		
		string log = "";
		
		foreach( KeyValuePair<int, BarStats> stat in barStats ) {
			
			// Bars held
			string line = stat.Key + ": ";
			line += ( stat.Value.getERatio() );

			log += ( line + "\n" );
		
		}
		
		Console.WriteLine( log );
		
	}
	
	public string getCSVRow() {
		string row = "";
		int dataCount = 0;

		foreach( KeyValuePair<int, BarStats> stat in barStats ) {
			
			if( dataCount == 0 ) {
				row += "," + stat.Value.getDataCount();
				dataCount = 1;
			}

			row += "," + stat.Value.getERatio();
			
			
		}
		
		return row;
	}
	
}
