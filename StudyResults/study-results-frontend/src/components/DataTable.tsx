import React, { useState, useEffect } from 'react';
import { apiService } from '../services/api';
import { StudyResult } from '../types/api';

interface DataTableProps {
  metric?: string;
  dateRange?: { from?: string; to?: string };
  participantId?: string;
  refreshTrigger?: number;
}

export const DataTable: React.FC<DataTableProps> = ({
  metric,
  dateRange,
  participantId,
  refreshTrigger,
}) => {
  const [data, setData] = useState<StudyResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  useEffect(() => {
    loadData();
  }, [metric, dateRange, participantId, refreshTrigger]);

  const loadData = async () => {
    setLoading(true);
    setError('');
    
    try {
      const results = await apiService.getStudyResults({
        metricName: metric,
        from: dateRange?.from,
        to: dateRange?.to,
        participantId: participantId,
      });
      setData(results);
      setCurrentPage(1); // Reset to first page when data changes
    } catch (error: any) {
      console.error('Error loading data:', error);
      setError('Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString();
  };

  const formatValue = (value: number) => {
    return value.toFixed(3);
  };

  if (loading) {
    return <div className="data-table loading">Loading data...</div>;
  }

  if (error) {
    return <div className="data-table error">{error}</div>;
  }

  
  const totalPages = Math.ceil(data.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentData = data.slice(startIndex, endIndex);

  const goToPage = (page: number) => {
    setCurrentPage(Math.max(1, Math.min(page, totalPages)));
  };

  return (
    <div className="data-table">
      <div className="table-header">
        <h3>Study Results Data</h3>
        <div className="table-info">
          Showing {startIndex + 1}-{Math.min(endIndex, data.length)} of {data.length} records
        </div>
      </div>
      
      {data.length === 0 ? (
        <div className="no-data">No data available</div>
      ) : (
        <>
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>Participant ID</th>
                  <th>Metric</th>
                  <th>Value</th>
                  <th>Timestamp</th>
                  <th>Category</th>
                  <th>Notes</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map((result) => (
                  <tr key={result.id}>
                    <td>{result.participantId}</td>
                    <td>{result.metricName}</td>
                    <td>{formatValue(result.metricValue)}</td>
                    <td>{formatDateTime(result.timestamp)}</td>
                    <td>{result.category || '-'}</td>
                    <td>{result.notes || '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          
          {totalPages > 1 && (
            <div className="pagination">
              <button
                onClick={() => goToPage(currentPage - 1)}
                disabled={currentPage === 1}
              >
                Previous
              </button>
              
              <span className="page-info">
                Page {currentPage} of {totalPages}
              </span>
              
              <button
                onClick={() => goToPage(currentPage + 1)}
                disabled={currentPage === totalPages}
              >
                Next
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
};