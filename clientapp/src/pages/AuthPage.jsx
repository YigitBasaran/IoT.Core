import React, { useState } from 'react';
import Navbar from './Navbar';

const AuthPage = () => {
    const [username, setUsername] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [oldPassword, setOldPassword] = useState('');
    const token = localStorage.getItem('token');

    const handleUpdatePassword = async () => {
        try {
            const res = await fetch('http://localhost:5000/api/auth/update-password', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`
                },
                body: JSON.stringify({ username, oldPassword, newPassword })
            });

            if (!res.ok) throw new Error('Failed to update password');
            alert('Password updated successfully!');
        } catch (err) {
            console.error(err);
            alert('Error updating password');
        }
    };

    return (
        <div>
            <Navbar />
            <div style={{ marginRight: '200px', padding: '2rem' }}>
                <h2>Update Password</h2>

                <div>
                    <label>Username:</label>
                    <input value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>

                <div>
                    <label>Old Password:</label>
                    <input
                        type="password"
                        value={oldPassword}
                        onChange={(e) => setOldPassword(e.target.value)}
                    />
                </div>

                <div>
                    <label>New Password:</label>
                    <input
                        type="password"
                        value={newPassword}
                        onChange={(e) => setNewPassword(e.target.value)}
                    />
                </div>

                <div style={{ marginTop: '1rem' }}>
                    <button onClick={handleUpdatePassword}>Update Password</button>
                </div>
            </div>
        </div>
    );
};

export default AuthPage;
