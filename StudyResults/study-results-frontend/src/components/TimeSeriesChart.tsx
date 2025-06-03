import React, { useState, useEffect } from 'react';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts';
import { apiService } from '../services/api';
import { TimeSeriesData } from '../types/api';

interface TimeSeriesChartProps {
  metric: string;
  dateRange?: { from?: string; to?: string };
}

export const TimeSeriesChart: React.FC<TimeSeriesChartProps> = ({
  metric,
  dateRange,
}) => {
  const [data, setData] = useState<TimeSeriesData[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    if (metric) {
      loadTimeSeriesData();
    }
  }, [metric, dateRange]);

  const loadTimeSeriesData = async () => {
    if (!metric) return;
    
    setLoading(true);
    setError('');
    
    try {
      const timeSeriesData = await apiService.getTimeSeriesData(
        metric,
        dateRange?.from,
        dateRange?.to
      );
      setData(timeSeriesData);
    } catch (error: any) {
      console.error('Error loading time series data:', error);
      setError('Failed to load chart data');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  if (loading) {
    return <div className="chart-container loading">Loading chart...</div>;
  }

  if (error) {
    return <div className="chart-container error">{error}</div>;
  }

  if (data.length === 0) {
    return <div className="chart-container">No data available for chart</div>;
  }

  const chartData = data.map(item => ({
    ...item,
    date: formatDate(item.timestamp),
    timestamp: item.timestamp,
  }));

  return (
    <div className="chart-container">
      <h3>Time Series - {metric}</h3>
      <ResponsiveContainer width="100%" height={400}>
        <LineChart data={chartData}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis 
            dataKey="date" 
            tick={{ fontSize: 12 }}
            angle={-45}
            textAnchor="end"
            height={80}
          />
          <YAxis tick={{ fontSize: 12 }} />
          <Tooltip 
            labelFormatter={(label, payload) => {
              if (payload && payload[0]) {
                return `Date: ${payload[0].payload.date}`;
              }
              return label;
            }}
            formatter={(value: number) => [value.toFixed(3), metric]}
          />
          <Legend />
          <Line
            type="monotone"
            dataKey="value"
            stroke="#8884d8"
            strokeWidth={2}
            dot={{ fill: '#8884d8', strokeWidth: 2, r: 4 }}
            activeDot={{ r: 6 }}
            name={metric}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};
