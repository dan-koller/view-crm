import { useNavigate } from 'react-router-dom'
import logo from '../images/crm-logo.png'

const Sidenav = () => {

    const navigate = useNavigate();

    return (
        <nav className="side-nav">
            <div className="logo-container" onClick={() => navigate('/')}>
                <img src={logo} alt="logo" />
            </div>
            <div className="controls-container">
                <div className="icon" onClick={() => navigate('/ticket')}>➕</div>
                <br />
                <div className="icon" onClick={() => navigate('/')}>❮❮</div>
            </div>
        </nav>
    );
}

export default Sidenav;