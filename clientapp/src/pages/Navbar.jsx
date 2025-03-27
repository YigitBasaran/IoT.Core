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
            background: '#1e1e1e', // Dark background
            borderLeft: '1px solid #333',
            transition: 'width 0.3s ease',
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'space-between',
            zIndex: 1000,
            color: '#fff'
        }}>
            <div>
                <button
                    onClick={() => setCollapsed(!collapsed)}
                    style={{
                        width: '100%',
                        padding: '10px',
                        backgroundColor: '#2c2c2c',
                        color: '#fff',
                        border: 'none',
                        cursor: 'pointer',
                        fontSize: '18px'
                    }}
                >
                    {collapsed ? '☰' : '⮞'}
                </button>

                <ul style={{ listStyle: 'none', padding: '1rem 0', margin: 0 }}>
                    {menuItems.map((item, index) => (
                        <li
                            key={index}
                            onClick={() => navigate(item.path)}
                            style={{
                                padding: '10px 16px',
                                cursor: 'pointer',
                                color: '#fff',
                                transition: 'background 0.2s ease',
                                whiteSpace: 'nowrap'
                            }}
                            onMouseEnter={(e) => e.target.style.background = '#333'}
                            onMouseLeave={(e) => e.target.style.background = 'transparent'}
                        >
                            {collapsed ? item.label[0] : item.label}
                        </li>
                    ))}
                </ul>
            </div>

            <button
                onClick={handleLogout}
                style={{
                    margin: '1rem',
                    padding: '10px',
                    background: '#b71c1c',
                    color: 'white',
                    border: 'none',
                    cursor: 'pointer',
                    fontWeight: 'bold',
                    borderRadius: '4px'
                }}
            >
                {collapsed ? '⏻' : 'Logout'}
            </button>
        </div>
    );
};

export default Navbar;
