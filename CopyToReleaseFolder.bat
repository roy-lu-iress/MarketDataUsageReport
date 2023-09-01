@echo off
Robocopy %cd% "\\tor-exch1\ydrive\Production Packages\EOD\Report\Other\MarketDataUsageReport" /E /R:0 /xf CopyToReleaseFolder.bat