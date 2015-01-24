#region Using statements
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using RightEdge.Common;
using RightEdge.Common.ChartObjects;
using RightEdge.Indicators;
#endregion

#region System class
public class MySystem : MySystemBase {
	
	public EdgeStats edgeStats = new EdgeStats();
	public string csvFilePath	=  "c:/Users/felix/Desktop/edgeRatio.csv";


	
	public override void Shutdown() {
		
		edgeStats.logData();
		edgeStats.writeCSV( csvFilePath );

	}
	
	
	
	/**
	* New bar: 
	* - First call newBar on MySymbolScript
	* - Then add the current day's strategy results (MFE and MAE) to edgeStats
	*/
	public override void NewBar() {
		
		base.NewBar();
		
		foreach( Position openPos in PositionManager.GetOpenPositions() ) {
			
			string strategyName = openPos.Description;
			PositionStats posStats = (PositionStats)openPos.Tag;

			edgeStats.addData( strategyName, openPos.BarsHeld, posStats.mfe / posStats.atr, posStats.mae / posStats.atr );
			
		}
		
	}
	
}
#endregion


public class MySymbolScript : MySymbolScriptBase
{

	
	/**
	* Use 
	* - 200 bars
	* - max 10 positions
	* - all futures
	* - 1.1.1990-31.12.2009
	*/


	public int maxPositions		= 10;
	public int holdingPeriod	= 200;

	
	public AverageTrueRange atr20;	
	public Random random;
	

	
	public override void Startup() {
		
		atr20 = new AverageTrueRange( 20 );
		random = new Random();
				
	}
	
	public void Shutdown() {
	}

	public override void NewBar() {
		
		if( Bars.Count < 40 ) {
			return;
		}
		
		Console.WriteLine( "open pos: {0}, maxPos {1}", getOpenPositionCount( "random", "long" ), maxPositions );
		Console.WriteLine( random.Next( 0, 20 ) );
		
		// Random
		if( getOpenPositionCount( "random", "short" ) < maxPositions ) {
			if( random.Next( 0, 20 ) == 19 ) {
				Console.WriteLine( "go long" );
				PositionManager.OpenPosition(  createPositionSettings( "random", "short" ) );
				PositionManager.OpenPosition(  createPositionSettings( "randomShort", "short" ) );
			}
		}
		
		if( getOpenPositionCount( "random", "long" ) < maxPositions ) {
			if( random.Next( 0, 20 ) == 19 ) {
				Console.WriteLine( "go short" );
				PositionManager.OpenPosition(  createPositionSettings( "random", "long" ) );
				PositionManager.OpenPosition(  createPositionSettings( "randomLong", "long" ) );
			}
		}
		
		
		
		// Collect statistics (ATR; MFE and MAE for all positions)
		IList<Position> openPositions = PositionManager.GetOpenPositions();
		foreach( Position openPos in openPositions) {
			
			if( openPos.Symbol == Symbol ) {
				this.collectStats( openPos );
			}
			
		}
	
	
		
		
	}
	
	
	
	/**
	* Checks if for strategyName AND Symbol AND type a position is already being held.
	* @returns <int>					Number of positions held for a ceratin symbol and strategy
	* @param <string> strategyName		Name of the strategy to return positions for
	* @param <string> posTypeName		Name of the position type ("long" or "short")
	*/
	private int getOpenPositionCount( string strategyName, string posTypeName ) {
		
		IList<Position> positions = PositionManager.GetOpenPositions();
		int openPositionCount = 0;
		
		PositionType posType = PositionType.Long;
		if( posTypeName == "short" ) {
			posType = PositionType.Short;
		}

		foreach( Position pos in positions ) {
			if( pos.Symbol == Symbol && pos.Description == strategyName && pos.Type == posType ) {
				openPositionCount++;
			}
		}
		
		return openPositionCount;

	}

	
	
	
	/**
	* Returns position settings for a strategy with name strategyName and position type posType
	* @param <string> strategyName			Name of the strategy to create position settings for
	* @param <string> posType				"long" or "short"
	*/
	private PositionSettings createPositionSettings( string strategyName, string posType ) {

		PositionSettings settings = new PositionSettings();
	
		settings.BarCountExit = holdingPeriod;
		settings.Description = strategyName;
		settings.OrderType = OrderType.Market;
		if( posType == "long" ) {
			settings.PositionType = PositionType.Long;
		}
		else {
			settings.PositionType = PositionType.Short;
		}
		settings.Size = 1;
		settings.Symbol = Symbol;
		
		return settings;
	
	}
	
	
	
	
	/**
	* Stores data for position: 
	* - atr (is only stored on init)
	* - mfe
	* - mae
	* Will be read in MySystemBase.NewSymbolBar
	*
	* @param <Position> position		Position to collect stats for
	*/
	public void collectStats( Position position ) {
	
		// Store current MAE and MFE on Position.Tag
		if( position.Tag == null ) {
			position.Tag = new PositionStats();
			((PositionStats)position.Tag).atr = atr20.Current;
		}
		
		PositionStats posStats = (PositionStats)position.Tag;
		
		double currentMfe, 
			   currentMae;
		
		if( position.Type == PositionType.Long ) {
			currentMfe = ( High.Current - position.EntryPrice.SymbolPrice );
			currentMae = ( position.EntryPrice.SymbolPrice - Low.Current );
		}
		else {
			currentMfe = ( position.EntryPrice.SymbolPrice - Low.Current );
			currentMae = ( High.Current - position.EntryPrice.SymbolPrice );
		}
		
		posStats.mfe = Math.Max( posStats.mfe, currentMfe );
		posStats.mae = Math.Max( posStats.mae, currentMae );
				
	}

}
