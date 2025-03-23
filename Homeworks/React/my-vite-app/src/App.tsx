import GetCatFactButton from './Components/GetCatFactButton.tsx';
import './App.css';

function App() {
  return (
    <div>
      <div>
        <GetCatFactButton btnName='Факты про кошкек' url='https://catfact.ninja/facts/' />
      </div>
      <div>
        <GetCatFactButton btnName='Ошибка' url='https://catfact.ninja/facts/1' />
      </div>
    </div>
  );
}

export default App;