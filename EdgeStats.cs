using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using RightEdge.Common;
using RightEdge.Indicators;


/**
* Statistics for all different strategies, e.g.:
* - donchain20: StrategyStats
* - macd: StrategyStats
* - donchainEma: StrategyStats
*/
public class EdgeStats {
	
	Dictionary<string, StrategyStats> edgeStats = new Dictionary<string, StrategyStats>();
	
	public void addData( string name, int bars, double mfe, double mae ) {
		
		//Console.WriteLine( "Add data for {0}, day {1}", name, bars );
		
		StrategyStats stat;
		if( edgeStats.TryGetValue( name, out stat ) ) {
			stat.addData( bars, mfe, mae );
		} 
		else {
			StrategyStats stratStats = new StrategyStats();
			stratStats.addData( bars, mfe, mae );
			edgeStats.Add( name, stratStats );
		}
		
	}
	
	public void logData() {
		
		foreach( KeyValuePair<string, StrategyStats> stat in edgeStats ) {
			Console.WriteLine( stat.Key );
			stat.Value.logData();
		}
		
	}
	
	public void writeCSV( string filePath ) {
		
		List<string> rows = new List<string>();
		int colCount = 0;

		// Compose rows
		foreach( KeyValuePair <string, StrategyStats> stat in edgeStats ) {
			string currentRow = stat.Key + stat.Value.getCSVRow();
			rows.Add( currentRow );
			
			if( colCount == 0 ) {
				colCount = stat.Value.barStats.Count;
			}
			
		}
		
		// First row
		string firstRow = "Symbol,Trades";
		for( int i = 0; i < colCount; i++ ) {
			firstRow += "," + ( i + 1 );
		}

		
		using( StreamWriter sw = new StreamWriter( filePath ) ) {
			sw.WriteLine( firstRow );
			foreach( String row in rows ) {
				sw.WriteLine( row );
			}
		}	
	
	}
	
}

