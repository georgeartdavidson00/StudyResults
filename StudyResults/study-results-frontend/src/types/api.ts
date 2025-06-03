export interface StudyResult {
  id: number;
  participantId: string;
  metricName: string;
  metricValue: number;
  timestamp: string;
  category?: string;
  notes?: string;
}

export interface SummaryStats {
  metricName: string;
  average: number;
  min: number;
  max: number;
  count: number;
  standardDeviation: number;
}

export interface TimeSeriesData {
  timestamp: string;
  value: number;
  metricName: string;
}

export interface UploadResponse {
  batchId: number;
  recordsProcessed: number;
  status: string;
  errors: string[];
}