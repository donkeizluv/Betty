<template>
    <v-app dark toolbar footer>
        <v-snackbar :timeout="5000"
            top
            :color="snackType"
            v-model="snackbar">
            {{ snackMessage }}
            <v-btn flat @click.native="snackbar = false">Đóng</v-btn>
        </v-snackbar>
        <v-toolbar color="primary">
            <v-toolbar-side-icon @click="drawer = true"></v-toolbar-side-icon>
            <v-toolbar-title class="white--text ">Betty</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-toolbar-items>
                <span></span>
                <v-btn v-show="isAuthenticated" flat>{{identity}}</v-btn>
                <v-btn v-show="isAuthenticated" @click="logout" flat>Logout</v-btn>
                </v-toolbar-items>
        </v-toolbar>
        <v-content>
            <template v-if="!isAuthenticated || !isAuthChecked">
                <login-modal :show="isAuthChecked"></login-modal>
            </template>
            <template v-else>
                <v-fade-transition mode='out-in'>
                    <view :is="currentView"
                        @error="showError"
                        @success="showSuccess"></view>
                </v-fade-transition>
            </template>
            <v-navigation-drawer
                    v-model="drawer"
                    temporary
                    absolute>
                <v-toolbar flat>
                    <v-list>
                        <v-list-tile>
                        <v-list-tile-title class="title">
                            Betty
                        </v-list-tile-title>
                        </v-list-tile>
                    </v-list>
                </v-toolbar>
                <v-divider></v-divider>
                <v-list>
                    <v-list-tile v-for="item in drawerItems" :key="item.title" @click="item.click(); toggleDrawer();">
                    <v-list-tile-action>
                        <v-icon>{{ item.icon }}</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                    </v-list-tile-content>
                    </v-list-tile>
                </v-list>
            </v-navigation-drawer>
        </v-content>
        <v-footer height="auto" class="grey darken-3">
            <v-layout row wrap justify-center>
                <v-btn v-for="link in links"
                       :key="link"
                       color="white"
                       flat>
                    {{ link }}
                </v-btn>
                <v-flex xs12 py-3 text-xs-center white--text>
                    &copy;2018 — <strong>HDSS</strong>
                </v-flex>
            </v-layout>
        </v-footer>
    </v-app>
</template>
<script>
    import Bet from './Bet.vue'
    import MyBets from './MyBets.vue'
    import LoginModal from './LoginModal.vue'
    import { CHECK_AUTH, LOGOUT } from '../actions'
    import axios from 'axios'

    export default {
        name: 'AppRoot',
        components: {
            'bet': Bet,
            'mybets': MyBets,
            'login-modal': LoginModal,
        },
        //Init state
        created: async function () {
            axios.interceptors.response.use(r => {
                return r;
            }, async e => {
                if(e.response.status == 401) {
                    await this.$store.dispatch(LOGOUT);
                }
                return Promise.reject(e);
                });
            //Check auth expire/reload auth
            //This better get called from nav guard
            if(!this.$store.getters.isAuthChecked)
                await this.$store.dispatch(CHECK_AUTH);
        },
        computed: {
            identity: function(){
                return (this.$store.getters.identity || '').toUpperCase();
            },
            isAuthenticated: function () {
                return this.$store.getters.isAuthenticated;
            },
            isAuthChecked: function () {
                return this.$store.getters.isAuthChecked;
            }
        },
        methods:{
            showError: function(mess){
                this.snackbar = true;
                this.snackType = "error";
                this.snackMessage = mess;
            },
            showSuccess: function(mess){
                this.snackbar = true;
                this.snackType = "success";
                this.snackMessage = mess;
            },
            showInfo: function(mess){
                this.snackbar = true;
                this.snackType = "info";
                this.snackMessage = mess;
            },
            logout: async function(){
                await this.$store.dispatch(LOGOUT);
            },
            toggleDrawer(){
                this.drawer = !this.drawer;
            }
        },
        data: function name() {
            const vm = this;
            return {
                links: [],
                snackbar: false,
                snackMessage: '',
                snackType: 'info',
                currentView: 'bet',
                drawer: false,
                drawerItems:[
                    {
                        icon: 'home',
                        title: 'Trang chính',
                        click() {
                            vm.currentView = 'bet';
                        }
                    },
                    {
                        icon: 'monetization_on',
                        title: 'Cược',
                        click() {
                            vm.currentView = 'mybets';
                        }
                    }
                ]
            }
        }
    }
</script>
<style>

</style>