import 'babel-polyfill'
import Vue from 'vue'
import App from './Component/App.vue'
import store from './store'
import 'vuetify/dist/vuetify.min.css'
import 'material-design-icons-iconfont/dist/material-design-icons.css'
import Vuetify from 'vuetify'

//https://vuetifyjs.com/en/theme-generator
Vue.use(Vuetify, {
    theme: {
        primary: "#FF5722",
        secondary: "#e57373",
        accent: "#9c27b0",
        error: "#f44336",
        warning: "#ffeb3b",
        info: "#2196f3",
        success: "#4caf50"
    }
});

new Vue({
    store: store,
    el: '#app',
    render: h => h(App)
});
