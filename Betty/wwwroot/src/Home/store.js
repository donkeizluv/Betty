import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";
import jwt from "jwt-decode";
import { ConstStorage } from "./AppConst";
import API from "./API";
import { HubConnectionBuilder } from '@aspnet/signalr'
import {
    CHECK_AUTH,
    LOGIN,
    LOGOUT,
    RELOAD_TOKEN,
    CLEAR_LOCALSTORE,
    INIT_LIVEFEED
} from "./actions";
import {
    AUTHENTICATED,
    AUTH_CHECKED,
    IDENTITY,
    EXPIRE,
    TOKEN,
    LOADING,
    LIVEFEED,
    LIVEFEED_ERROR
} from "./mutations";

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        //Auth
        authChecked: false, //Auth check on app boot
        authenticated: false,
        token: localStorage.getItem(ConstStorage.TokenStorage) || null,
        identity: localStorage.getItem(ConstStorage.IdentityStorage) || null,
        expire: localStorage.getItem(ConstStorage.ExpireStorage) || 0,
        //Loading
        isLoading: false,
        livefeed: null,
        liveFeedError: false

    },
    getters: {
        //Auth
        isAuthenticated: state => state.authenticated,
        isAuthChecked: state => state.authChecked,
        identity: state => state.identity,
        //Other
        isLoading: state => state.isLoading,
        livefeed: state => state.livefeed,
        livefeedError: state => state.liveFeedError
    },
    mutations: {
        //Auth
        [AUTHENTICATED](state, value) {
            state.authenticated = value;
        },
        [AUTH_CHECKED](state, value) {
            state.authChecked = value;
        },
        [TOKEN](state, value) {
            state.token = value;
        },
        [EXPIRE](state, value) {
            state.expire = value;
        },
        [IDENTITY](state, value) {
            state.identity = value;
        },
        //App wide loading
        [LOADING](state, value) {
            state.isLoading = value;
        },
        [LIVEFEED](state, value){
            state.livefeed = value;
        },
        [LIVEFEED_ERROR](state, value){
            state.liveFeedError = value;
        }
    },
    actions: {
        [CHECK_AUTH]: async ({ commit, dispatch, state }) => {
            if (!state.token) {
                //If no token then no need to check
                commit(AUTH_CHECKED, true);
                commit(AUTHENTICATED, false);
                return;
            }
            //Start checking
            commit(AUTH_CHECKED, false);
            try {
                await axios({
                    url: API.Ping,
                    headers: { Authorization: `Bearer ${state.token}` }
                });
                //Reload auth states
                await dispatch(RELOAD_TOKEN);
            } catch (e) {
                await dispatch(LOGOUT);
            } finally {
                //Check done
                commit(AUTH_CHECKED, true);
            }
        },
        [LOGIN]: async ({ commit, dispatch }, cred) => {
            commit(LOADING, true);
            try {
                let { username, pwd } = cred;
                let form = new FormData();
                form.append("username", username);
                form.append("pwd", pwd);
                let { data } = await axios.post(API.Login, form);
                let token = data.auth_token;
                //Store token & not in token info
                localStorage.setItem(ConstStorage.TokenStorage, token);
                localStorage.setItem(ConstStorage.IdentityStorage, username);
                localStorage.setItem(
                    ConstStorage.ExpireStorage,
                    data.expires_in
                );
                //init states
                await dispatch(RELOAD_TOKEN);
                commit(LOADING, false);
            } catch (e) {
                //console.log(e);
                commit(LOADING, false);
                await dispatch(CLEAR_LOCALSTORE);
                throw e;
            }
        },
        //Call this to init app using stored token
        [RELOAD_TOKEN]: async ({ commit, dispatch }) => {
            //Get token from store
            let token = localStorage.getItem(ConstStorage.TokenStorage);
            let identity = localStorage.getItem(ConstStorage.IdentityStorage);
            let exp = localStorage.getItem(ConstStorage.ExpireStorage);

            //Check
            if (token == undefined || identity == undefined || exp < 1)
                throw new Error("Fail to load token from storage");
            let { sub } = jwt(token);
            //init states
            commit(TOKEN, token);
            commit(EXPIRE, exp);
            commit(IDENTITY, sub);
            //Set token to axios
            axios.defaults.headers.common["Authorization"] = "Bearer " + token;
            commit(AUTHENTICATED, true);
            await dispatch(INIT_LIVEFEED);
        },
        [LOGOUT]: async ({ commit, dispatch }) => {
            //Clear axios
            delete axios.defaults.headers.common["Authorization"];
            //Clear all state and storage
            commit(TOKEN, null);
            commit(EXPIRE, 0);
            commit(IDENTITY, null);
            await dispatch(CLEAR_LOCALSTORE);
            commit(AUTHENTICATED, false);
        },
        [CLEAR_LOCALSTORE]: async () => {
            localStorage.removeItem(ConstStorage.TokenStorage);
            localStorage.removeItem(ConstStorage.IdentityStorage);
            localStorage.removeItem(ConstStorage.ExpireStorage);
        },
        [INIT_LIVEFEED]: async({ commit }) =>{
            try {
                let signalr = new HubConnectionBuilder()
                    .configureLogging({ 
                        log(level) {
                            if(level > 3)
                                commit(LIVEFEED_ERROR, true);            
                        }
                    })
                    .withUrl("/Livefeed")
                    .build();
                await signalr.start();
                commit(LIVEFEED, signalr);
                commit(LIVEFEED_ERROR, false);
            } catch (error) {
                console.log(error);
                commit(LIVEFEED_ERROR, true);
            }
        }
    }
});
