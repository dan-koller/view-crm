import { useState, useEffect, useContext } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import axios from 'axios'
import CategoriesContext from '../context'
import authService from '../components/api-authorization/AuthorizeService'

const TicketPage = ({ editMode }) => {

    // TODO: Check if owner attribute needs to be added to the ticket model
    const [formData, setFormData] = useState({
        owner: '',
        status: 'not started',
        progress: 0,
        timestamp: new Date().toISOString(),
    });

    const { categories, setCategories } = useContext(CategoriesContext);

    const navigate = useNavigate();
    let { id } = useParams();

    const handleChange = (e) => {
        const value = e.target.value;
        const name = e.target.name;

        setFormData((prevState) => ({
            ...prevState,
            [name]: value
        }))
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (editMode) {
            try {
                const response = await axios.put(`https://localhost:5002/api/ticket/${id}`, formData);
                const success = response.status === 204;
                if (success) {
                    navigate('/dashboard');
                }
            } catch (error) {
                // Handle any errors
                console.error("Error during PUT request:", error);
            }
        }

        if (!editMode) {
            console.log('posting');
            try {
                const response = await axios.post('https://localhost:5002/api/ticket', formData);

                const success = response.status === 201;
                if (success) {
                    navigate('/dashboard');
                }
            } catch (error) {
                // Handle any errors
                console.error("Error during POST request:", error);
            }
        }
    }

    const fetchData = async () => {
        const response = await axios.get(`https://localhost:5002/api/ticket/${id}`)
        console.log('Response', response)
        setFormData(response.data)
    }

    useEffect(() => {
        if (editMode) {
            fetchData()
        }

        // Get the user name from the auth service
        async function fetchUserName() {
            try {
                const user = await Promise.resolve(authService.getUser());
                const userName = user.name;
                setFormData((prevFormData) => ({ ...prevFormData, owner: userName }));
            } catch (error) {
                console.error(error);
            }
        }

        fetchUserName();

    }, [])

    return (
        <div className="ticket">
            <h1>{editMode ? 'Update Your Ticket' : 'Create a Ticket'}</h1>
            <div className="ticket-container">
                <form onSubmit={handleSubmit}>
                    <section>
                        <label htmlFor="title">Title</label>
                        <input
                            id="title"
                            name="title"
                            type="text"
                            onChange={handleChange}
                            required={true}
                            value={formData.title}
                        />

                        <label htmlFor="description">Description</label>
                        <input
                            id="description"
                            name="description"
                            type="text"
                            onChange={handleChange}
                            required={true}
                            value={formData.description}
                        />

                        <label>Category</label>
                        <select
                            name="category"
                            value={formData.category}
                            onChange={handleChange}
                        >
                            {categories?.map((category, _index) => (
                                <option value={category}>{category}</option>
                            ))}
                        </select>

                        <label htmlFor="new-category">New Category</label>
                        <input
                            id="new-category"
                            name="category"
                            type="text"
                            onChange={handleChange}
                            value={formData.category}
                        />

                        <label>Priority</label>
                        <div className="multiple-input-container">
                            <input
                                id="priority-1"
                                name="priority"
                                type="radio"
                                onChange={handleChange}
                                value={1}
                                checked={formData.priority == 1}
                            />
                            <label htmlFor="priority-1">1</label>
                            <input
                                id="priority-2"
                                name="priority"
                                type="radio"
                                onChange={handleChange}
                                value={2}
                                checked={formData.priority == 2}
                            />
                            <label htmlFor="priority-2">2</label>
                            <input
                                id="priority-3"
                                name="priority"
                                type="radio"
                                onChange={handleChange}
                                value={3}
                                checked={formData.priority == 3}
                            />
                            <label htmlFor="priority-3">3</label>
                            <input
                                id="priority-4"
                                name="priority"
                                type="radio"
                                onChange={handleChange}
                                value={4}
                                checked={formData.priority == 4}
                            />
                            <label htmlFor="priority-4">4</label>
                            <input
                                id="priority-5"
                                name="priority"
                                type="radio"
                                onChange={handleChange}
                                value={5}
                                checked={formData.priority == 5}
                            />
                            <label htmlFor="priority-5">5</label>
                        </div>

                        {editMode && (
                            <>
                                <input
                                    type="range"
                                    id="progress"
                                    name="progress"
                                    value={formData.progress}
                                    min="0"
                                    max="100"
                                    onChange={handleChange}
                                />
                                <label htmlFor="progress">Progress</label>

                                <label>Status</label>
                                <select
                                    name="status"
                                    value={formData.status}
                                    onChange={handleChange}
                                >
                                    <option selected={formData.status == 'done'} value="done">
                                        Done
                                    </option>
                                    <option
                                        selected={formData.status == 'working on it'}
                                        value="working on it"
                                    >
                                        Working on it
                                    </option>
                                    <option selected={formData.status == 'stuck'} value="stuck">
                                        Stuck
                                    </option>
                                    <option
                                        selected={formData.status == 'not started'}
                                        value="not started"
                                    >
                                        Not Started
                                    </option>
                                </select>
                            </>
                        )}
                        <input type="submit" />
                    </section>

                    <section>
                        <label htmlFor="owner">Owner</label>
                        <input
                            id="owner"
                            name="owner"
                            type="owner"
                            onChange={handleChange}
                            required={true}
                            value={formData.owner}
                        />

                        <label htmlFor="avatar">Avatar</label>
                        <input
                            id="avatar"
                            name="avatar"
                            type="url"
                            onChange={handleChange}
                            value={formData.avatar}
                        />
                        <div className="img-preview">
                            {formData.avatar && (
                                <img src={formData.avatar} alt="avatar preview" />
                            )}
                        </div>
                    </section>
                </form>
            </div>
        </div>
    )
}

export default TicketPage