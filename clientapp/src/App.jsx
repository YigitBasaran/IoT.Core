import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/LoginPage.jsx';
import DashboardPage from './pages/DashboardPage.jsx';
import ClientsPage from './pages/ClientsPage.jsx';
import DevicesPage from './pages/DevicesPage.jsx';
import DataPage from './pages/DataPage.jsx';
import AuthPage from './pages/AuthPage.jsx';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LoginPage />} />
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/clients" element={<ClientsPage />} />
                <Route path="/devices" element={<DevicesPage />} />
                <Route path="/data" element={<DataPage />} />
                <Route path="/auth" element={<AuthPage />} />
            </Routes>
        </Router>
    );
}

export default App;
