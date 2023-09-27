import { useState, useEffect, useContext } from "react";
import { useNavigate, useParams } from "react-router-dom";
import CategoriesContext from "../context";
import { useAuth } from "../components/auth/AuthContext";
import { api } from "../components/misc/Api";

const TicketPage = ({ editMode }) => {
    const [formData, setFormData] = useState({
        status: "not started",
        progress: 0,
        timestamp: new Date().toISOString(),
        closed: false,
    });

    // Save the current formdata in a variable
    const [currentFormData, setCurrentFormData] = useState(formData);

    const { categories, setCategories } = useContext(CategoriesContext);

    const navigate = useNavigate();
    let { id } = useParams();

    const auth = useAuth();

    const handleChange = (e) => {
        const value = e.target.value;
        const name = e.target.name;

        setFormData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (editMode) {
            try {
                const response = await api.updateTicket(formData, id); // Api.js already handles the bearerAuth
                const success = response.status === 204;

                if (success) navigate("/dashboard");
            } catch (error) {
                // Handle any errors
                console.error("Error during PUT request:", error);
            }
        }

        if (!editMode) {
            console.log("posting");
            try {
                const response = await api.postTicket(formData);
                const success = response.status === 201;

                if (success) navigate("/dashboard");
            } catch (error) {
                // Handle any errors
                console.error("Error during POST request:", error);
            }
        }
    };

    const fetchData = async () => {
        const response = await api.fetchTicket(id);
        console.log("Response", response);
        setFormData(response.data);
    };

    const fetchUserName = async () => {
        try {
            const user = await Promise.resolve(auth.getUser());
            const userName = user.email;
            setFormData((prevFormData) => ({
                ...prevFormData,
                owner: userName,
            }));
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        editMode ? fetchData() : fetchUserName();
    }, []);

    // This is a workaround to avoid unintentional data carryover when switching from /ticket/:id to /ticket/new
    useEffect(() => {
        if (!editMode) {
            // Save the current form data in a variable
            setCurrentFormData(formData);

            // Reset the form data
            setFormData({
                ...formData,
                title: "",
                description: "",
                category: "",
                priority: 1,
                // If the owner is already populated, don't reset it
                owner: formData.owner ? formData.owner : "",
                notes: "",
                avatar: "",
            });
        } else {
            // If going back to edit mode, restore the previous form data
            setFormData(currentFormData);
        }
    }, [editMode]); // ESLint wants me to add formData to the dependency array, but that would cause an infinite loop of updates and the fields cannot be edited

    return (
        <div className='ticket'>
            <h1>{editMode ? "Update Your Ticket" : "Create a Ticket"}</h1>
            <div className='ticket-container'>
                <form onSubmit={handleSubmit}>
                    <section>
                        <label htmlFor='title'>Title</label>
                        <input
                            id='title'
                            name='title'
                            type='text'
                            onChange={handleChange}
                            required={true}
                            value={formData.title}
                        />

                        <label htmlFor='description'>Description</label>
                        <input
                            id='description'
                            name='description'
                            type='text'
                            onChange={handleChange}
                            required={true}
                            value={formData.description}
                        />

                        <label>Category</label>
                        <select
                            name='category'
                            value={formData.category}
                            onChange={handleChange}
                        >
                            {categories?.map((category, _index) => (
                                <option value={category}>{category}</option>
                            ))}
                        </select>

                        <label htmlFor='new-category'>New Category</label>
                        <input
                            id='new-category'
                            name='category'
                            type='text'
                            onChange={handleChange}
                            value={formData.category}
                        />

                        <label>Priority</label>
                        <div className='multiple-input-container'>
                            <input
                                id='priority-1'
                                name='priority'
                                type='radio'
                                onChange={handleChange}
                                value={1}
                                checked={formData.priority == 1}
                            />
                            <label htmlFor='priority-1'>1</label>
                            <input
                                id='priority-2'
                                name='priority'
                                type='radio'
                                onChange={handleChange}
                                value={2}
                                checked={formData.priority == 2}
                            />
                            <label htmlFor='priority-2'>2</label>
                            <input
                                id='priority-3'
                                name='priority'
                                type='radio'
                                onChange={handleChange}
                                value={3}
                                checked={formData.priority == 3}
                            />
                            <label htmlFor='priority-3'>3</label>
                            <input
                                id='priority-4'
                                name='priority'
                                type='radio'
                                onChange={handleChange}
                                value={4}
                                checked={formData.priority == 4}
                            />
                            <label htmlFor='priority-4'>4</label>
                            <input
                                id='priority-5'
                                name='priority'
                                type='radio'
                                onChange={handleChange}
                                value={5}
                                checked={formData.priority == 5}
                            />
                            <label htmlFor='priority-5'>5</label>
                        </div>

                        {editMode && (
                            <>
                                <input
                                    type='range'
                                    id='progress'
                                    name='progress'
                                    value={formData.progress}
                                    min='0'
                                    max='100'
                                    onChange={handleChange}
                                />
                                <label htmlFor='progress'>Progress</label>

                                <label>Status</label>
                                <select
                                    name='status'
                                    value={formData.status}
                                    onChange={handleChange}
                                >
                                    <option
                                        selected={formData.status == "done"}
                                        value='done'
                                    >
                                        Done
                                    </option>
                                    <option
                                        selected={
                                            formData.status == "working on it"
                                        }
                                        value='working on it'
                                    >
                                        Working on it
                                    </option>
                                    <option
                                        selected={formData.status == "stuck"}
                                        value='stuck'
                                    >
                                        Stuck
                                    </option>
                                    <option
                                        selected={
                                            formData.status == "not started"
                                        }
                                        value='not started'
                                    >
                                        Not Started
                                    </option>
                                </select>

                                {/*Hidden field for new tickets. Closed is false by default*/}
                                <label htmlFor='closed' type='hidden' />
                                <input
                                    id='closed'
                                    name='closed'
                                    type='hidden'
                                    onChange={handleChange}
                                    value={formData.closed}
                                />
                            </>
                        )}
                        <input type='submit' />
                    </section>

                    <section>
                        <label htmlFor='owner'>Owner</label>
                        <input
                            id='owner'
                            name='owner'
                            type='owner'
                            onChange={handleChange}
                            required={true}
                            value={formData.owner}
                        />
                        {/* If in edit mode, add a button to assign the ticket to the current user */}
                        {editMode && (
                            <>
                                <label htmlFor='update'>Update owner</label>
                                <button
                                    onClick={(e) => {
                                        e.preventDefault();
                                        setFormData((prevFormData) => ({
                                            ...prevFormData,
                                            // If the owner is the current user, don't update it
                                            owner:
                                                auth.getUser().email ==
                                                formData.owner
                                                    ? formData.owner
                                                    : auth.getUser().email,
                                            // Reset avatar only if the owner is not the current user
                                            avatar:
                                                auth.getUser().email ==
                                                formData.owner
                                                    ? formData.avatar
                                                    : "",
                                        }));
                                    }}
                                >
                                    Assign to me
                                </button>
                            </>
                        )}

                        {/* Add a textfield to add notes to a ticket */}
                        <label htmlFor='notes'>Notes</label>
                        <textarea
                            id='notes'
                            name='notes'
                            onChange={handleChange}
                            value={formData.notes}
                        />

                        <label htmlFor='avatar'>Avatar</label>
                        <input
                            id='avatar'
                            name='avatar'
                            type='url'
                            onChange={handleChange}
                            value={formData.avatar}
                        />
                        <div className='img-preview'>
                            {formData.avatar && (
                                <img
                                    src={formData.avatar}
                                    alt='avatar preview'
                                />
                            )}
                        </div>
                    </section>
                </form>
            </div>
        </div>
    );
};

export default TicketPage;
