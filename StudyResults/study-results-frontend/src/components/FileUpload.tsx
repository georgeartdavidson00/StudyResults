import React, { useState } from 'react';
import { apiService } from '../services/api';

interface FileUploadProps {
  onUploadSuccess: () => void;
}

export const FileUpload: React.FC<FileUploadProps> = ({ onUploadSuccess }) => {
  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [message, setMessage] = useState<string>('');
  const [messageType, setMessageType] = useState<'success' | 'error' | 'info'>('info');

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = event.target.files?.[0];
    if (selectedFile) {
      if (selectedFile.name.endsWith('.csv')) {
        setFile(selectedFile);
        setMessage('');
      } else {
        setMessage('Please select a CSV file');
        setMessageType('error');
        setFile(null);
      }
    }
  };

  const handleUpload = async () => {
    if (!file) {
      setMessage('Please select a file');
      setMessageType('error');
      return;
    }

    setUploading(true);
    setMessage('Uploading file...');
    setMessageType('info');

    try {
      const response = await apiService.uploadCsv(file);
      
      if (response.status === 'Success') {
        setMessage(`Successfully uploaded ${response.recordsProcessed} records`);
        setMessageType('success');
        setFile(null);
        // Reset file input
        const fileInput = document.getElementById('file-input') as HTMLInputElement;
        if (fileInput) fileInput.value = '';
        
        onUploadSuccess();
      } else {
        setMessage(`Upload failed: ${response.errors.join(', ')}`);
        setMessageType('error');
      }
    } catch (error: any) {
      console.error('Upload error:', error);
      setMessage(`Upload failed: ${error.response?.data?.message || error.message}`);
      setMessageType('error');
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="upload-container">
      <h2>Upload Study Results</h2>
      <div className="upload-form">
        <div className="file-input-container">
          <input
            id="file-input"
            type="file"
            accept=".csv"
            onChange={handleFileChange}
            disabled={uploading}
          />
          <label htmlFor="file-input" className="file-input-label">
            {file ? file.name : 'Choose CSV file'}
          </label>
        </div>
        
        <button
          onClick={handleUpload}
          disabled={!file || uploading}
          className="upload-button"
        >
          {uploading ? 'Uploading...' : 'Upload'}
        </button>
      </div>
      
      {message && (
        <div className={`message ${messageType}`}>
          {message}
        </div>
      )}
      
      <div className="upload-info">
        <h3>CSV Format Requirements:</h3>
        <p>Your CSV file should contain the following columns:</p>
        <ul>
          <li><strong>ParticipantId</strong> (required): Unique identifier for each participant</li>
          <li><strong>MetricName</strong> (required): Name of the metric being measured</li>
          <li><strong>MetricValue</strong> (required): Numeric value of the metric</li>
          <li><strong>Timestamp</strong> (required): Date and time of the measurement</li>
          <li><strong>Category</strong> (optional): Category or group classification</li>
          <li><strong>Notes</strong> (optional): Additional notes or comments</li>
        </ul>
      </div>
    </div>
  );
};