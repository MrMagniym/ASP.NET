import { useState } from "react"
import { Button } from 'react-bootstrap';
import apiClient from "../api/Client";
import WeatherItem from "./WeatherItem";
import WeatherForecast from "./WeatherForecast";

const WeatherForecastButton: React.FC = () => {
    const [weatherItems, setWeatherItems] = useState<WeatherItem[]>([]);

    const fetchWeather = async () => {
        try {
            const response = await apiClient.get<WeatherItem[]>('WeatherForecast');
            if (response.status >= 200 && response.status < 300)
                setWeatherItems(response.data);
            else 
            throw new Error("Ошибка получения погоды");
        } catch (error) {
            console.error('Ошибка получения погоды: ', error);
        }
    };

    return (
        <div>
            <div>
                <Button onClick={fetchWeather}>Show Weather</Button>
            </div>
            <div>
                {weatherItems && weatherItems.length > 0 && <WeatherForecast items={weatherItems} />}
            </div>
        </div>
    );
};

export default WeatherForecastButton;
