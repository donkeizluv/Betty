<template>
    <v-flex class="ma-1" lg5 md6 sm10 xs12>
        <v-card class="text-center cdgrey darken-2">
            <v-card-title class="pa-2 pt-3">
                <v-layout justify-center align-center row wrap>
                    <!-- <v-flex d-inline-flex xs6>
                        <counter :to="game.Start"
                            :from="now"
                            @timedout="timedOutTimer"/>
                    </v-flex>                         -->
                    <v-flex d-inline-flex justify-center align-center class="text-start" lg3 md4 xs5>
                        <v-checkbox
                            class="checkbox"
                            v-model="player1"
                            :disabled="reg || timerStop"
                            color="red"
                            value="red"
                            hide-details></v-checkbox>
                        <span style="min-width: 25px" class="title flag-icon" :class="'flag-icon-' + game.CountryCode1"></span>
                        <span class="subheading country">{{game.Player1}}</span>
                    </v-flex>
                    <v-flex xs2>
                        <!-- <span class="vs-size flag-icon vs"></span> -->
                        <img class="vs-size flag-icon vs" src="/static/vs.svg">
                    </v-flex>
                    <v-flex d-inline-flex justify-center align-center lg3 md4 xs5>
                        <span class="subheading country">{{game.Player2}}</span>
                        <span style="min-width: 25px" class="title flag-icon" :class="'flag-icon-' + game.CountryCode2"></span>
                        <v-checkbox
                            class="checkbox"
                            v-model="player2"
                            :disabled="reg || timerStop"
                            color="red"
                            value="red"
                            hide-details></v-checkbox>
                    </v-flex>
                </v-layout>
            </v-card-title>
            <v-card-text class="pt-0">
                <v-divider class="mb-2"></v-divider>
                <v-spacer></v-spacer>
                <v-layout class="ma-3 text-center" justify-center row>
                    <v-flex d-flex lg6 md9 xs10>
                        <counter :to="game.Start"
                            :from="now"
                            @timedout="timedOutTimer"/>
                    </v-flex>
                </v-layout>
                <v-layout class="text-center" justify-center align-center row wrap mt-2>
                   <v-flex d-inline-flex lg1 md4 xs4>
                       <span class="caption prefix">Chấp:</span>
                   </v-flex>
                   <v-flex d-inline-flex lg4 md6 xs8>
                       <up-down class="title" :status="odds1Trend" :value="game.Odds1"/>
                       <span class="title grey--text text--lighten-2 ml-1 mr-1">|</span>
                       <up-down class="title" :status="odds2Trend" :value="game.Odds2" :show-before="false"/>
                   </v-flex>
                   <v-flex d-inline-flex lg1 md4 xs4>
                       <span class="caption prefix">H/A:</span>
                   </v-flex>
                   <v-flex d-inline-flex lg4 md6 xs8>
                       <up-down class="title" :status="win1Trend" :value="game.Win1"/>
                       <span class="title grey--text text--lighten-2 ml-1 mr-1">|</span>
                       <up-down class="title" :status="win2Trend" :value="game.Win2" :show-before="false"/>
                   </v-flex>
                </v-layout>
            </v-card-text>
            <v-card-actions class="pa-0">
                <v-layout class="text-center" justify-center align-center row wrap>
                    <v-flex lg6 md6 sm6 xs7>
                        <v-slider 
                            v-model="amt"
                            :disabled="reg || timerStop"
                            thumb-color="orange darken-3"
                            track-color="orange darken-3"
                            label="$"
                            thumb-label
                            :min="minBet"
                            :max="maxBet"
                            :step="step"/>
                    </v-flex>
                    <v-flex lg3 md3 xs4 justify-end class="pt-0">
                        <span class="subheading cash">{{formatAmt}} đ</span>
                    </v-flex>
                    <v-flex class="mb-3 text-center" justify-center d-inline-flex xs12>
                        <v-flex class="text-center" lg4 xs6>
                            <v-btn @click="bet" :disabled="!canSubmit" :loading="betting" color="success">
                                {{buttonText}}
                            </v-btn>
                        </v-flex>
                        <v-flex class="text-center" lg4 xs6>
                            <v-btn @click="cancel" :disabled="!canCancel" :loading="canceling" color="error">
                                Hủy
                            </v-btn>
                        </v-flex>
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
        "now":{
            type: String,
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
        buttonText(){
            if(this.reg) return "Đã đăng ký";
            if(this.timerStop) return "Hết giờ";
            return "Đăng ký";
        },
        reg(){
            return this.game.Registered;
        },
        canSubmit(){
            //Not reged || timed out & must select player
            if(this.betting || this.canceling) return false;
            if(this.reg || this.timerStop) return false;
            return this.player1 || this.player2;
        },
        canCancel(){
            if(this.betting || this.canceling) return false;
            if(!this.reg || this.timerStop) return false;
            return true;
        },
        p1Percentage(){
            if(this.game.TotalReg == 0) return 0;
            return this.game.TotalReg / this.game.Player1Reg;
        },
        p2Percentage(){
            if(this.game.TotalReg == 0) return 0;
            return this.game.TotalReg / this.game.Player2Reg;
        },
        formatAmt() {
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
            downColor: 'purple lighten-2',
            betting: false,
            canceling: false

        };
    },
    watch: {
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
                this.betting = true;
                //Post rq
                await axios.post(API.CreateBet, bet);
                //Set reg to true
                this.game.Registered = true;
                this.$emit("success","Đăng ký thành công!");
                this.betting = false;
            } catch (error) {
                // throw error;
                this.$emit("error","Đăng ký thất bại!");
                this.betting = false;
            }
        },
        cancel: async function(){
            if(!this.canCancel) return;
            try {
                let cancel = {
                    Id: this.game.Id
                };
                this.canceling = true;
                this.$emit('cancel', cancel);
                //Post rq
                await axios.post(API.CancelBet, cancel);
                //Set reg to true
                this.game.Registered = false;
                this.$emit("success","Hủy đăng ký thành công!");
                this.canceling = false;
            } catch (error) {
                // throw error;
                this.$emit("error","Hủy thất bại!");
                this.canceling = false;
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
    width: 3.25rem;
    height: 3.25rem;
}
.vs {
    /* background-image: url(/static/vs.svg); */
    filter: invert(.5) sepia(1) saturate(20) hue-rotate(10deg);
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
.text-center{
    text-align: center;
}
.text-end{
    text-align: end;
}
.text-start{
    text-align: start;
}
.checkbox{
    height: 25px;
    width: 25px;
}
.no-wrap{
    flex-wrap: wrap-reverse;
}
</style>