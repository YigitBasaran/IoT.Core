import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const LoginPage = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [showSetPassword, setShowSetPassword] = useState(false);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleLogin = async () => {
        try {
            const response = await fetch('http://localhost:5000/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) throw new Error('Invalid credentials');

            const data = await response.json();
            localStorage.setItem('token', data.token);
            setError('');
            navigate('/dashboard');
        } catch (err) {
            setError(err.message);
        }
    };

    const handleSetPassword = async () => {
        try {
            const res = await fetch('http://localhost:5000/api/auth/set-password', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, newPassword })
            });

            if (!res.ok) throw new Error('Failed to set password');
            alert('Password set successfully! You can now log in.');
            setShowSetPassword(false);
        } catch (err) {
            console.error(err);
            alert('Error setting password');
        }
    };

    return (
        <div style={{ padding: '2rem' }}>
            <h2>Plan-S Operator Login</h2>

            <input
                type="text"
                placeholder="Username"
                value={username}
                onChange={e => setUsername(e.target.value)}
            /><br /><br />

            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={e => setPassword(e.target.value)}
            /><br /><br />

            <button onClick={handleLogin}>Login</button>
            <button onClick={() => setShowSetPassword(!showSetPassword)} style={{ marginLeft: '10px' }}>
                {showSetPassword ? 'Cancel' : 'Set Initial Password'}
            </button>

            {error && <p style={{ color: 'red' }}>{error}</p>}

            {showSetPassword && (
                <div style={{ marginTop: '2rem' }}>
                    <h4>Set Initial Password</h4>
                    <input
                        type="password"
                        placeholder="New Password"
                        value={newPassword}
                        onChange={e => setNewPassword(e.target.value)}
                    /><br /><br />
                    <button onClick={handleSetPassword}>Set Password</button>
                </div>
            )}
        </div>
    );
};

export default LoginPage;
