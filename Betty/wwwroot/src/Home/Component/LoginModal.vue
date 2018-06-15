<template>
    <v-dialog scrollable persistent v-model="show" max-width="400px" width="400px">
        <v-card>
            <v-card-title>
                <span class="headline">Đăng nhập</span>
            </v-card-title>
            <v-card-text>
                <v-layout row>
                    <v-flex xs12>
                        <v-text-field v-model="username"
                                label="Username"
                                @keyup.enter.native="login"
                        ></v-text-field>
                    </v-flex>
                </v-layout>
                <v-layout row>
                    <v-flex xs12>
                        <v-text-field label="Pass"
                                v-model="pwd"
                                @keyup.enter.native="login"
                                :append-icon="e1 ? 'visibility' : 'visibility_off'"
                                :append-icon-cb="() => (e1 = !e1)"
                                :type="e1 ? 'password' : 'text'"
                                counter></v-text-field>
                    </v-flex>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="orange darken-2" flat="flat" @click="login" :disabled="!canLogin">Login</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import { LOGIN } from '../actions'
export default {
    name: 'LoginModal',
    props: {
        show: {
            required: true
        },
        redirect: {
            default: 'https://www.google.com'
        }
    },
    computed: {
        canLogin: function(){
            return this.username && this.pwd;
        }
    },
    data: function name() {
        return{
            username: "",
            pwd: "",
            e1: true
        }
    },
    methods: {
        login: async function(){
            if(!this.canLogin) return;
            await this.$store.dispatch(LOGIN, {username: this.username, pwd: this.pwd});
            this.username = "";
            this.pwd = "";
        }
    }
}
</script>