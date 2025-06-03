import React, { useState, useEffect } from 'react';
import { apiService } from '../services/api';
import { SummaryStats } from '../types/api';

interface SummaryStatsCardProps {
  metric: string;
  dateRange?: { from?: string; to?: string };
}

export const SummaryStatsCard: React.FC<SummaryStatsCardProps> = ({
  metric,
  dateRange,
}) => {
  const [stats, setStats] = useState<SummaryStats | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    if (metric) {
      loadStats();
    }
  }, [metric, dateRange]);

  const loadStats = async () => {
    if (!metric) return;
    
    setLoading(true);
    setError('');
    
    try {
      const summaryStats = await apiService.getSummaryStats(
        metric,
        dateRange?.from,
        dateRange?.to
      );
      setStats(summaryStats);
    } catch (error: any) {
      console.error('Error loading stats:', error);
      setError('Failed to load statistics');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="stats-card loading">Loading statistics...</div>;
  }

  if (error) {
    return <div className="stats-card error">{error}</div>;
  }

  if (!stats) {
    return <div className="stats-card">No data available</div>;
  }

  return (
    <div className="stats-card">
      <h3>Summary Statistics - {stats.metricName}</h3>
      <div className="stats-grid">
        <div className="stat-item">
          <span className="stat-label">Average:</span>
          <span className="stat-value">{stats.average.toFixed(3)}</span>
        </div>
        <div className="stat-item">
          <span className="stat-label">Minimum:</span>
          <span className="stat-value">{stats.min.toFixed(3)}</span>
        </div>
        <div className="stat-item">
          <span className="stat-label">Maximum:</span>
          <span className="stat-value">{stats.max.toFixed(3)}</span>
        </div>
        <div className="stat-item">
          <span className="stat-label">Count:</span>
          <span className="stat-value">{stats.count}</span>
        </div>
        <div className="stat-item">
          <span className="stat-label">Std Dev:</span>
          <span className="stat-value">{stats.standardDeviation.toFixed(3)}</span>
        </div>
      </div>
    </div>
  );
};