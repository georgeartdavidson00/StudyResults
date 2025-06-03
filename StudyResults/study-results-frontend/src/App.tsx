import React, { useState } from 'react';
import { FileUpload } from './components/FileUpload';
import { MetricSelector } from './components/MetricSelector';
import { SummaryStatsCard } from './components/SummaryStatsCard';
import { TimeSeriesChart } from './components/TimeSeriesChart';
import { DataTable } from './components/DataTable';
import { DateRangeFilter } from './components/DateRangeFilter';
import './App.css';

function App() {
  const [selectedMetric, setSelectedMetric] = useState<string>('');
  const [dateRange, setDateRange] = useState<{ from?: string; to?: string }>({});
  const [refreshTrigger, setRefreshTrigger] = useState<number>(0);

  const handleUploadSuccess = () => {
    // Trigger refresh of all components
    setRefreshTrigger(prev => prev + 1);
  };

  const handleFromDateChange = (date: string) => {
    setDateRange(prev => ({ ...prev, from: date }));
  };

  const handleToDateChange = (date: string) => {
    setDateRange(prev => ({ ...prev, to: date }));
  };

  const handleClearDateRange = () => {
    setDateRange({});
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Study Results Dashboard</h1>
        <p>Upload CSV files and analyze study results with interactive charts and statistics</p>
      </header>

      <main className="App-main">
        <section className="upload-section">
          <FileUpload onUploadSuccess={handleUploadSuccess} />
        </section>

        <section className="analysis-section">
          <div className="controls-row">
            <MetricSelector
              selectedMetric={selectedMetric}
              onMetricChange={setSelectedMetric}
            />
            <DateRangeFilter
              from={dateRange.from}
              to={dateRange.to}
              onFromChange={handleFromDateChange}
              onToChange={handleToDateChange}
              onClear={handleClearDateRange}
            />
          </div>

          {selectedMetric && (
            <>
              <div className="stats-section">
                <SummaryStatsCard
                  metric={selectedMetric}
                  dateRange={dateRange}
                />
              </div>

              <div className="chart-section">
                <TimeSeriesChart
                  metric={selectedMetric}
                  dateRange={dateRange}
                />
              </div>

              <div className="table-section">
                <DataTable
                  metric={selectedMetric}
                  dateRange={dateRange}
                  refreshTrigger={refreshTrigger}
                />
              </div>
            </>
          )}
        </section>
      </main>

      <footer className="App-footer">
        <p>Study Results Dashboard - Built with React and ASP.NET Core</p>
      </footer>
    </div>
  );
}

export default App;