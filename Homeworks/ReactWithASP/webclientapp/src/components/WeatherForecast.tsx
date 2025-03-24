import React from "react";
import WeatherItem from "./WeatherItem";

interface WeatherProps {
    items: WeatherItem[];
}

const WeatherForecast: React.FC<WeatherProps> = ({ items }) => {
    return (
      <div className="weather-table">

        <div className="weather-table-row">
          <div className="weather-table-header">Date</div>
          <div className="weather-table-header">TemperatureC</div>
          <div className="weather-table-header">TemperatureF</div>
          <div className="weather-table-header">Summary</div>
        </div>
  
        {items.map((w, index) => (
          <div key={index} className="weather-table-row">
            <div className="weather-table-cell">
              {new Date(w.date).toLocaleDateString()}
            </div>
            <div className="weather-table-cell numeric">
              {w.temperatureC}
            </div>
            <div className="weather-table-cell numeric">
              {w.temperatureF}
            </div>
            <div className="weather-table-cell">
              {w.summary}
            </div>
          </div>
        ))}

      </div>
    );
  };

export default WeatherForecast;
