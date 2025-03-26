import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const Navbar = () => {
    const [collapsed, setCollapsed] = useState(false);
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/');
    };

    const menuItems = [
        { label: 'Dashboard', path: '/dashboard' },
        { label: 'Clients', path: '/clients' },
        { label: 'Devices', path: '/devices' },
        { label: 'Data', path: '/data' },
        { label: 'Auth', path: '/auth' },
    ];

    return (
        <div style={{
            position: 'fixed',
            top: 0,
            right: 0,
            width: collapsed ? '60px' : '200px',
            height: '100vh',
            background: '#f5f5f5',
            borderLeft: '1px solid #ddd',
            transition: 'width 0.3s ease',
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'space-between',
            zIndex: 1000
        }}>
            <div>
                <button onClick={() => setCollapsed(!collapsed)} style={{ width: '100%' }}>
                    {collapsed ? '☰' : '⮞'}
                </button>
                <ul style={{ listStyle: 'none', padding: '1rem 0' }}>
                    {menuItems.map((item, index) => (
                        <li key={index} style={{ padding: '10px', cursor: 'pointer' }}
                            onClick={() => navigate(item.path)}>
                            {collapsed ? item.label[0] : item.label}
                        </li>
                    ))}
                </ul>
            </div>

            <button onClick={handleLogout} style={{
                margin: '1rem',
                padding: '10px',
                background: 'red',
                color: 'white',
                border: 'none',
                cursor: 'pointer'
            }}>
                {collapsed ? '⏻' : 'Logout'}
            </button>
        </div>
    );
};

export default Navbar;
