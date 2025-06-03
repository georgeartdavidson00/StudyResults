import React, { useState, useEffect } from 'react';
import { apiService } from '../services/api';

interface MetricSelectorProps {
  selectedMetric: string;
  onMetricChange: (metric: string) => void;
}

export const MetricSelector: React.FC<MetricSelectorProps> = ({
  selectedMetric,
  onMetricChange,
}) => {
  const [metrics, setMetrics] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadMetrics();
  }, []);

  const loadMetrics = async () => {
    try {
      const availableMetrics = await apiService.getAvailableMetrics();
      setMetrics(availableMetrics);
      
      // Auto-select first metric if none selected
      if (availableMetrics.length > 0 && !selectedMetric) {
        onMetricChange(availableMetrics[0]);
      }
    } catch (error) {
      console.error('Error loading metrics:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div>Loading metrics...</div>;
  }

  if (metrics.length === 0) {
    return <div>No metrics available. Please upload some data first.</div>;
  }

  return (
    <div className="metric-selector">
      <label htmlFor="metric-select">Select Metric:</label>
      <select
        id="metric-select"
        value={selectedMetric}
        onChange={(e) => onMetricChange(e.target.value)}
      >
        <option value="">-- Select a metric --</option>
        {metrics.map((metric) => (
          <option key={metric} value={metric}>
            {metric}
          </option>
        ))}
      </select>
    </div>
  );
};