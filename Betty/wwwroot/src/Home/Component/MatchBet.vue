<template>
    <v-flex class="pa-2" d-flex lg6>
        <v-card class="grey darken-2">
            <v-card-title class="pa-0">
                <v-container>
                    <v-layout justify-center align-center row wrap>
                        <v-flex lg1>
                            <v-checkbox
                                v-model="player1"
                                :disabled="reg || timerStop"
                                color="red"
                                value="red"
                                hide-details></v-checkbox>
                        </v-flex>
                        <v-flex lg3>
                            <span style="min-width: 30px" class="headline flag-icon" :class="'flag-icon-' + game.CountryCode1"></span>
                            <span class="headline country">{{game.Player1}}</span>
                        </v-flex>
                        <v-flex lg2>
                            <img class="vs-size" src="/static/vs.png">
                        </v-flex>
                        <v-flex lg3>
                            <span class="headline country">{{game.Player2}}</span>
                            <span style="min-width: 30px" class="headline flag-icon" :class="'flag-icon-' + game.CountryCode2"></span>
                        </v-flex>
                        <v-flex lg1>
                            <v-checkbox
                                v-model="player2"
                                :disabled="reg || timerStop"
                                color="red"
                                value="red"
                                hide-details></v-checkbox>
                        </v-flex>
                    </v-layout>
                    <v-divider></v-divider>
                </v-container>
            </v-card-title>
            <v-card-text>
                <v-layout justify-center row>
                    <v-flex lg10>
                        <counter :date="game.Start"
                            prefix="Còn lại:"
                            @timedout="timedOutTimer"/>
                    </v-flex>
                </v-layout>
                <v-layout justify-center align-center row mt-4>
                   <v-flex lg1>
                       <span class="body-2 prefix">Chấp:</span>
                   </v-flex>
                   <v-flex lg4>
                       <up-down class="title" :status="odds1Trend" :value="game.Odds1"/>
                       <span class="title grey--text text--lighten-2 ml-1 mr-1">|</span>
                       <up-down class="title" :status="odds2Trend" :value="game.Odds2" :show-before="false"/>
                   </v-flex>
                   <v-flex lg1>
                       <span class="body-2 prefix">Ăn thua:</span>
                   </v-flex>
                   <v-flex lg4>
                       <up-down class="title" :status="win1Trend" :value="game.Win1"/>
                       <span class="title grey--text text--lighten-2 ml-1 mr-1">|</span>
                       <up-down class="title" :status="win2Trend" :value="game.Win2" :show-before="false"/>
                   </v-flex>
                </v-layout>
            </v-card-text>
            <v-card-actions class="pt-0">
                <v-layout justify-center align-center row wrap>
                    <v-flex lg7 md8 xs10>
                        <v-slider v-model="amt"
                            :disabled="reg || timerStop"
                            thumb-color="orange darken-3"
                            track-color="orange darken-3"
                            label="Cược"
                            thumb-label
                            :min="minBet"
                            :max="maxBet"
                            :step="step"></v-slider>
                    </v-flex>
                    <v-flex lg3 md4 xs5>
                        <span class="title cash">{{formatAmt}} VND</span>
                    </v-flex>
                    <v-flex lg4 offset-lg2 xs2>
                        <v-btn @click="bet" :disabled="!canSubmit" color="success">{{buttonText}}</v-btn>
                    </v-flex>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-flex>
</template>
<script>
import myFooter from './MyFooter.vue'
import LoginModal from './LoginModal.vue'
import CountDown from './CountDown.vue'
import UpDown from './UpDown.vue'
import API from '../API'
import axios from 'axios'
export default {
    name: 'MatchBet',
    components:{
        'counter': CountDown,
        'up-down': UpDown
    },
    props:{
        "game":{
            type: Object,
            required: true
        },
        "maxBet":{
            type: Number,
            required: true
        },
        "minBet":{
            type: Number,
            required: true
        },
        "step":{
            type: Number,
            required: true
        },
        "confirm":{
            type: Boolean,
            required: true
        },
        "regBefore": {
            type: Number,
            default: 2
        }
    },
    computed: {
        buttonText: function(){
            if(this.reg) return "Đã đăng ký";
            if(this.timerStop) return "Hết giờ";
            return "Đăng ký";
        },
        reg: function(){
            return this.game.Registered;
        },
        canSubmit: function(){
            //Not reged || timed out & must select player
            if(this.reg || this.timerStop) return false;
            return this.player1 || this.player2;
        },
        formatAmt: function() {
            return this.amt.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    },
    data: function(){
        return {
            amt: 0,
            player1: false,
            player2: false,
            timerStop: false,
            //Trend status
            odds1Trend: null,
            odds2Trend: null,
            win1Trend: null,
            win2Trend: null,
            upColor: 'lime lighten-2',
            downColor: 'purple lighten-2'
        };
    },
    watch: {
        // game: {
        //     handler: function(oldVal, newVal){
        //         console.log('game changed...');
        //         console.log(oldVal);
        //         console.log(newVal);
        //     },
        //     deep: true
        // },
        'game.Odds1': function(newVal, oldVal) {
        //   console.log('Odds1 changed..');
          this.odds1Trend = newVal > oldVal;
        },
        'game.Odds2': function(newVal, oldVal) {
        //   console.log('Odds2 changed..');
          this.odds2Trend = newVal > oldVal;
        },
        'game.Win1': function(newVal, oldVal) {
        //   console.log('Win1 changed..');
          this.win1Trend = newVal > oldVal;
        },
        'game.Win2': function(newVal, oldVal) {
        //   console.log('Win2 changed..');
          this.win2Trend = newVal > oldVal;
        },
        player1: function (newVal, oldVal) {
            if(newVal)
                this.player2 = false;
        },
        player2: function (newVal, oldVal) {
            if(newVal)
                this.player1 = false;
        }
    },
    methods:{
        bet: async function(){
            if(!this.canSubmit) return;
            
            try {
                let bet = {
                    Id: this.game.Id,
                    Amt: this.amt,
                    Player: this.player1? 1 : 2
                };
                this.$emit('bet', bet);
                //Confirm to continue
                if(!this.confirm) return;
                //Post rq
                await axios.post(API.CreateBet, bet);
                //Set reg to true
                this.game.Registered = true;
                this.$emit("success","Đăng ký chơi thành công!");

            } catch (error) {
                // throw error;
                this.$emit("error","Đăng ký thất bại!");
            }
        },
        timedOutTimer: function(){
            this.timerStop = true;
        }
    }
}
</script>
<style scoped>
.vs-size{
    max-width: 60px;
    max-height: 60px;
}
.country{
    color: #FFB74D
}
.stats{
    color: #FF6D00
}
.prefix{
    color: #E0E0E0;
}
.cash{
    color: #FFB74D;
}
</style>