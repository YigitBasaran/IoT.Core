import React, { useEffect, useState } from 'react';

const ClientsPage = () => {
    const [clients, setClients] = useState([]);
    const [authUsers, setAuthUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [role, setRole] = useState('Client');
    const token = localStorage.getItem('token');

    const fetchData = async () => {
        try {
            const [clientRes, userRes] = await Promise.all([
                fetch('http://localhost:5000/api/client', {
                    headers: { Authorization: `Bearer ${token}` }
                }),
                fetch('http://localhost:5000/api/auth', {
                    headers: { Authorization: `Bearer ${token}` }
                })
            ]);

            const clientsData = await clientRes.json();
            const usersData = await userRes.json();

            const merged = clientsData.map((client) => {
                const matchedUser = usersData.find(
                    (user) => user.username === client.name
                );
                return {
                    ...client,
                    role: matchedUser?.role || 'N/A'
                };
            });

            setClients(merged);
            setAuthUsers(usersData);
            setLoading(false);
        } catch (err) {
            console.error('Failed to fetch data:', err);
        }
    };

    const addClient = async () => {
        if (!name.trim() || !email.trim() || !role.trim()) {
            alert('Name, Email and Role are required!');
            return;
        }

        try {
            const response = await fetch('http://localhost:5000/api/client', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`
                },
                body: JSON.stringify({ name, email, role: role === "Client" ? 1 : 0 })
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Add client failed:', errorText);
                throw new Error('Failed to add client');
            }

            setName('');
            setEmail('');
            setRole('Client'); 
            fetchData();
        } catch (err) {
            console.error(err);
            alert('Could not add client');
        }
    };

    const deleteClient = async (id) => {
        if (!window.confirm('Are you sure you want to delete this client?')) return;

        try {
            await fetch(`http://localhost:5000/api/client/${id}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });
            fetchData();
        } catch (err) {
            console.error('Failed to delete client:', err);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    if (loading) return <p style={{ padding: '2rem' }}>Loading clients...</p>;

    return (
        <div style={{ padding: '2rem' }}>
            <h2>Registered Clients</h2>

            <div style={{ marginBottom: '1rem' }}>
                <input
                    placeholder="Name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
                <input
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    style={{ marginLeft: '10px' }}
                />
                <select value={role} onChange={(e) => setRole(e.target.value)} style={{ marginLeft: '10px' }}>
                    <option value="Client">Client</option>
                    <option value="Operator">Operator</option>
                </select>
                <button onClick={addClient} style={{ marginLeft: '10px' }}>Add Client</button>
            </div>

            <table border="1" cellPadding="8" style={{ borderCollapse: 'collapse' }}>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Email (Username)</th>
                        <th>Role</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {clients.map((client) => (
                        <tr key={client.id}>
                            <td>{client.name}</td>
                            <td>{client.email}</td>
                            <td>{client.role}</td>
                            <td>
                                <button onClick={() => deleteClient(client.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default ClientsPage;
