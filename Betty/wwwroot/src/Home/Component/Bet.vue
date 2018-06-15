<template id="bet">
   <v-container fluid>
            <v-dialog v-model="dialog" max-width="290">
                <v-card>
                    <v-card-title class="headline">Cảnh báo!</v-card-title>
                    <v-card-text>Livefeed tỉ lệ đã tắt, có thể tỉ lệ ko phải là mới nhất. Bạn hãy cân nhắc cẩn thận.</v-card-text>
                    <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="red darken-1" flat @click.native="dialog = false; confirm = true;">Chơi luôn</v-btn>
                    <v-btn color="green darken-1" @click.native="dialog = false;">Nghỉ</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
            <v-layout class="mb-2" justify-end row>
                <v-flex flex lg1 md1 sm2 xs5>
                    <span v-if="livefeedError">Livefeed: <b>OFF</b></span>
                    <span v-else>
                        Livefeed: <b>ON</b>
                    </span>
                </v-flex>
                <v-flex lg1 md1 sm1 xs5>
                    <spinner v-show="showSpinner" width="5px" height="10px"></spinner>
                </v-flex>
            </v-layout>
            <v-layout justify-center row wrap>
                <template v-for="game in gameList">
                    <match-bet :game.sync="game"
                            :minBet="minBet"
                            :maxBet="maxBet"
                            :step="step"
                            :key="game.Id"
                            :confirm="confirm"
                            @error="emitError"
                            @success="emitSuccess"
                            @bet="userBet"/>
                </template>
            </v-layout>
    </v-container>
</template>
<script>
import Spinner from 'vue-spinner/src/ScaleLoader.vue'
import matchBet from './MatchBet.vue'
import axios from 'axios'
import API from '../API'
export default {
    name: 'Bet',
    template: '#bet',
    created: async function(){
        await this.init();
        //Subscribe to feed
        if(this.livefeed) {
            // console.log('Listen to feed..');
            this.livefeed.on("Fixtures", this.livefeedCallback);
        }
    },
    beforeDestroy: function(){
        if(this.livefeed) {
            // console.log('Stop listening to feed..');
            this.livefeed.off("Fixtures", this.livefeedCallback);
        }
    },
    components: {
        'match-bet': matchBet,
        'spinner' : Spinner
    },
    data: function () {
        return {
            maxBet: 0,
            minBet: 0,
            step: 0,
            gameList: [],
            dialog: false,
            confirm: true
        }
    },
    watch:{
        livefeedError(value){
            //If no livefeed then confirm needed
            this.confirm = !value;
        }
    },
    computed: {
        livefeed(){
            return this.$store.getters.livefeed;
        },
        livefeedError(){
            return this.$store.getters.livefeedError;
        },
        showSpinner(){
            return !this.livefeedError;
        }
    },
    methods: {
        init: async function(){
            try {
                var { data } = await axios.get(API.BetVM);
                this.gameList = data.Games;
                this.maxBet = data.MaxBet;
                this.minBet = data.MinBet;
                this.step = data.Step;
            } catch (error) {
                // Alert error...
                this.emitError("Ko tải được dữ liệu :(");
            }
        },
        userBet(bet) {
            //Warn if making bet when livefeed is off
            if(!this.confirm){
                this.dialog = true;
            }
                
        },
        emitError: function(mess){
            this.$emit("error", mess);
        },
        emitSuccess: function(mess){
            this.$emit("success", mess);
        },
        livefeedCallback: function(games){
            // console.log(games);
            //Update data
            games.forEach(g => {
                var game = this.gameList.find(game => game.Id == g.Id);
                if(game){
                    game.Odds1 = g.Odds1;
                    game.Odds2 = g.Odds2;
                    game.Win1 = g.Win1;
                    game.Win2 = g.Win2;
                }
            });
        }
    }
}
</script>
<style scoped>

</style>