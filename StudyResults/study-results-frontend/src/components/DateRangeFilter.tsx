import React from 'react';

interface DateRangeFilterProps {
  from?: string;
  to?: string;
  onFromChange: (date: string) => void;
  onToChange: (date: string) => void;
  onClear: () => void;
}

export const DateRangeFilter: React.FC<DateRangeFilterProps> = ({
  from,
  to,
  onFromChange,
  onToChange,
  onClear,
}) => {
  return (
    <div className="date-range-filter">
      <h4>Date Range Filter</h4>
      <div className="date-inputs">
        <div className="date-input">
          <label htmlFor="from-date">From:</label>
          <input
            id="from-date"
            type="datetime-local"
            value={from || ''}
            onChange={(e) => onFromChange(e.target.value)}
          />
        </div>
        <div className="date-input">
          <label htmlFor="to-date">To:</label>
          <input
            id="to-date"
            type="datetime-local"
            value={to || ''}
            onChange={(e) => onToChange(e.target.value)}
          />
        </div>
        <button onClick={onClear} className="clear-button">
          Clear
        </button>
      </div>
    </div>
  );
};
